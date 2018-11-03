using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraSystem
{
    public partial class CameraController : MonoBehaviour
    {
        bool m_bColliding;
        float m_fAdjustedRotationX, m_fOriginRotationX, m_fAdjustedDistance, m_fOriginTargetDistance;
        Vector3[] m_vAdjustedClipPoints, m_vDesiredClipPoints;

        void RefreshCameraClipPoints(Vector3 vCameraPosition, Quaternion qCameraRotation, ref Vector3[] vIntoArray)
        {
            if (m_Cam == null)
            {
                Debug.LogWarning("Camera missing!");
                return;
            }
            //清空ClipPoint array
            vIntoArray = new Vector3[5];

            //算出每個ClipPoint
            float z = m_Cam.nearClipPlane;
            float x = Mathf.Tan(m_Cam.fieldOfView / 3.41f) * z;
            float y = x / m_Cam.aspect;

            //左上
            vIntoArray[0] = qCameraRotation * new Vector3(-x, y, z) + vCameraPosition;
            //左下
            vIntoArray[1] = qCameraRotation * new Vector3(x, y, z) + vCameraPosition;
            //右上
            vIntoArray[2] = qCameraRotation * new Vector3(-x, -y, z) + vCameraPosition;
            //右下
            vIntoArray[3] = qCameraRotation * new Vector3(x, -y, z) + vCameraPosition;
            //攝影機位置
            vIntoArray[4] = vCameraPosition - transform.forward * z;
        }

        bool CollisionDetectedAtClipPoints(Vector3[] vClipPoints, Vector3 vTargetPosition)
        {
            //ClipPoint的四個角跟中心點都要能夠看到目標，依序做射線檢查
            for (int i = 0; i < vClipPoints.Length; i++)
            {
                Ray ray = new Ray(vTargetPosition, vClipPoints[i] - vTargetPosition);
                float fDistance = Vector3.Distance(vClipPoints[i], vTargetPosition);
                if (Physics.Raycast(ray, fDistance, CollisionLayer))
                {
                    return true;
                }
            }
            return false;
        }

        float GetAdjustedDistanceFromTarget(Vector3 vTarget)
        {
            //有障礙物的話，回傳最短沒有障礙物的距離

            float fDistance = -1f;
            for (int i = 0; i < m_vDesiredClipPoints.Length; i++)
            {
                Vector3 vHitDirection = m_vDesiredClipPoints[i] - vTarget;
                Ray ray = new Ray(vTarget, vHitDirection);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, vHitDirection.magnitude, CollisionLayer))
                {
                    if (fDistance == -1)
                    {
                        fDistance = hitInfo.distance;
                    }
                    else
                    {
                        if (hitInfo.distance < fDistance)
                        {
                            fDistance = hitInfo.distance;
                        }
                    }
                }
            }
            return fDistance == -1 ? fDistFromTarget : fDistance;
        }

        void CheckColliding(Vector3 vTargetPosition)
        {
            if (CollisionDetectedAtClipPoints(m_vDesiredClipPoints, vTargetPosition) == true)
            {
                if (m_bColliding == false)
                {
                    OnObstacleDetected();
                }
                m_bColliding = true;

            }
            else
            {
                m_bColliding = false;
            }
        }

        void OnObstacleDetected()
        {

            StartCoroutine(WaitForNotColliding());
        }

        IEnumerator WaitForNotColliding()
        {
            m_fOriginRotationX = fXRotation;
            m_fOriginTargetDistance = fDistFromTarget;
            yield return new WaitUntil(() => m_bColliding == false);
            fXRotation = m_fOriginRotationX;
            fDistFromTarget = m_fOriginTargetDistance;
        }

        void AdjustDestination()
        {
            if (m_bColliding == true)
            {
                m_fAdjustedDistance = Mathf.Min(GetAdjustedDistanceFromTarget(m_vTargetPos), fMaxZoom);
                m_fAdjustedRotationX = m_fAdjustedDistance >= fMinZoom
                    ? Mathf.Clamp(fXRotation + 30f, fMinXRotation, fMaxXRotation)
                    : fXRotation;
                if (m_fAdjustedDistance < fMinZoom) m_fAdjustedDistance = fMinZoom;
            }
        }















        void TransparentObstacle(Collider collision)
        {
            float dist = Vector3.Distance(collision.gameObject.transform.position, transform.position);
            Renderer[] renderers;
            renderers = collision.gameObject.GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                HashSet<Material> materials = new HashSet<Material>();
                foreach (var r in renderers)
                {
                    materials.Add(r.material);
                }

                foreach (var m in materials)
                {
                    if (m.HasProperty("_Color"))
                    {
                        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        m.SetInt("_ZWrite", 0);
                        m.DisableKeyword("_ALPHATEST_ON");
                        m.DisableKeyword("_ALPHABLEND_ON");
                        m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        m.renderQueue = 3000;
                        float taAlpha = Mathf.Lerp(1f, fAlpha, dist / fRadius);
                        float offset = taAlpha - m.color.a;
                        if (offset != 0)
                        {
                            float alpha = m.color.a + offset * fFadeTime * Time.deltaTime;
                            m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
                        }
                        else
                            m.color = new Color(m.color.r, m.color.g, m.color.b, taAlpha);
                    }
                }
            }
        }

        IEnumerator FadeBack(Renderer[] renderers)
        {
            HashSet<Material> materials = new HashSet<Material>();
            foreach (var r in renderers)
            {
                materials.Add(r.material);
            }
            foreach (var m in materials)
            {
                if (m.HasProperty("_Color"))
                {
                    while (m.color.a != 1f)
                    {
                        float alpha = m.color.a + 10 * Time.deltaTime;
                        if (alpha > 1) alpha = 1;
                        m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    m.SetInt("_ZWrite", 1);
                    m.DisableKeyword("_ALPHATEST_ON");
                    m.DisableKeyword("_ALPHABLEND_ON");
                    m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    m.renderQueue = -1;
                }
            }
        }

        void ReverseTransparency(Collider collision)
        {
            Renderer[] renderers = collision.gameObject.GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                StartCoroutine(FadeBack(renderers));
            }
        }
    }

}

