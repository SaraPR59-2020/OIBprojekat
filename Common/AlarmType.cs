using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public enum AlarmType
    {
        [EnumMember]
        BEZ_ALARMA,
        [EnumMember]
        LAŽNI_ALARM,
        [EnumMember]
        INFORMACIJA,
        [EnumMember]
        UPOZORENJE,
        [EnumMember]
        GREŠKA
    }

}
