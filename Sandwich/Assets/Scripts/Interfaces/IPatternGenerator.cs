/// <summary>
/// Interface that can be implemented for creating a new generation logic.
/// </summary>
public interface IPatternGenerator
{
    void GeneratePattern();
    bool UnfoldIngredients(Node node);
}