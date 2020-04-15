using System;

/// <summary>
/// Interface can be implemented by any class that wants to be loaded into the command buffer.
/// </summary>
public interface ICommand
{
   void Execute();
   void Undo(float speed);

   String FlipperName();
}