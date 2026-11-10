using UnityEngine;

public interface IGameAction
{
    string Name { get; }
    void Execute();
    void Undo();
}
public class MoveAction : IGameAction
{
    private readonly Transform target;
    private readonly Vector3 delta;

    public string Name => "Mover";

    public MoveAction(Transform target, Vector3 delta)
    {
        this.target = target;
        this.delta = delta;
    }

    public void Execute() { target.position += delta; }
    public void Undo() { target.position -= delta; }
}
public class ScaleAction : IGameAction
{
    private readonly Transform target;
    private readonly float factor;

    public string Name => "Agrandar";

    public ScaleAction(Transform target, float factor)
    {
        this.target = target;
        this.factor = factor;
    }

    public void Execute() { target.localScale *= factor; }
    public void Undo() { target.localScale /= factor; }
}
public class ColorAction : IGameAction
{
    private readonly Renderer target;
    private readonly Color newColor;
    private Color previousColor;

    public string Name => "Cambiar color";

    public ColorAction(Renderer target, Color newColor)
    {
        this.target = target;
        this.newColor = newColor;
    }
    public void Execute()
    {
        previousColor = target.material.color;
        target.material.color = newColor;
    }
    public void Undo() { target.material.color = previousColor; }
}