using System;

//  �Ԃ�l�Ȃ��̊֐����s�N���X
public class ActionCommand : ICommand
{
    Action _Action;    //  �֐��ۗL�ϐ�

    //  �R���X�g���N�^
    public ActionCommand(Action Action)
    {
        _Action = Action;
    }
    //  �֐����s
    public object Execute()
    {
        _Action();
        return null;
    }
}
