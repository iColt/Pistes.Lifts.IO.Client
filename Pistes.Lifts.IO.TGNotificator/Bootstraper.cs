using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pistes.Lifts.IO.TGNotificator;

public interface IBootstraper
{
    void Initialize();
}

public class Bootstraper : IBootstraper
{
    public void Initialize()
    {
        throw new NotImplementedException();
    }
}
