using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GF.Service.FandP.PriceCalc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPriceCalcService" in both code and config file together.
    [ServiceContract]
    public interface IPriceCalcService
    {
        [OperationContract]
        bool CalculatePrice(string xmlRequest, out string xmlResponse, out string error);
    }
}
