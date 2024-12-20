namespace ArtHub;

public abstract class BaseController
{
    public virtual async Task<IActionResult> ExecuteAsync(string actionName, object parameters)
    {
        var method = GetType().GetMethod(actionName);
        if (method == null)
        {
            throw new Exception($"Action '{actionName}' not found.");
        }
        var result = method.Invoke(this, new[] { parameters });
        
        if (result is Task<IActionResult> taskResult)
        {
            return await taskResult;
        }

        throw new Exception($"Action '{actionName}' must return Task<IActionResult>.");    }
}
