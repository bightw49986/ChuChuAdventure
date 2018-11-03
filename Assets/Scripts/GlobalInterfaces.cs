using System;

public interface IMoveable
{
    event Action Moved;
    void OnMoved();
}
