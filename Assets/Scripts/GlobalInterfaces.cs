using System;

public interface IMoveable
{
    event Action Moved;
    void OnMoved();
}

public interface ISingleton
{
    void SingletonLized();
}