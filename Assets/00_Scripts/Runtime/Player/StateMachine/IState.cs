using System;

public interface IState : IDisposable
{
    void OnEnter();
    void Update();
    void FixedUpdate();
    void OnExit();
}   
