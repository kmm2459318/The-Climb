using Zenject;

//  フェード関係のバインド
public class FadingInstalelr : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IDownFading>().To<FadeController>().FromComponentInHierarchy().AsSingle();
    }
}
