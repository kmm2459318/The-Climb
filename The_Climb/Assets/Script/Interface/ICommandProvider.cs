using System.Collections.Generic;

//    �R�}���h�^�����񋟃v���o�C�_�[
public interface ICommandProvider
{
    Dictionary<KickerMoveCommander.KickerCommanderMethod, ICommand> GetCommandMap();
}
