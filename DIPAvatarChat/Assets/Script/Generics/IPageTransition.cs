using System.Collections;

public interface IPageTransition
{
    public abstract void FadeInUI();
    public abstract IEnumerator ExitRoutine();
}
