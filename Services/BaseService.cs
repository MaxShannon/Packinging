using DbEfModel;

namespace Services
{
    public class BaseService
    {
        public readonly int _lxId; // 8
        public readonly int _lxId2; // 9
        public readonly int _lxCheckId; // 7

        public BaseService()
        {
            _lxId = 8;
            _lxId2 = 9;
            _lxCheckId = 7;
        }

        public bool IsInOut(sys_ry user)
        {
            if (user.lx == _lxId || user.lx == _lxId2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}