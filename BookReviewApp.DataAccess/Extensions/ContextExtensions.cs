namespace BookReviewApp.DataAccess.Extensions;

public static class ContextExtensions
{
    public static void UpdateEntity<T>(this Context context, T toUpdate, T updated) where T : class
    {
        context.Entry(toUpdate!).CurrentValues.SetValues(updated);
    }
}