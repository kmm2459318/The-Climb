using NUnit.Framework.Internal;
using System;

//  �Ԃ�l����̊֐����s�N���X
public class FuncCommand<T> : ICommand
{
    Func<T> _Func;    //  

    //  �R���X�g���N�^
    public FuncCommand(Func<T> Func)
    {
        _Func = Func;
    }

    public object Execute() => _Func();    //  �֐��̕Ԃ�l���擾
}
