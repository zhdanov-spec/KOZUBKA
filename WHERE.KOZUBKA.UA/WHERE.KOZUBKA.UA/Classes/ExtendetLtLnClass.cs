using GoogleMapsComponents.Maps;
using ua.kozubka.context.Models.Where;

namespace ua.kozubka.where.Classes
{
    public class ExtendetLtLnClass: LatLngLiteral
    {
       public WhereTeamDetail WhereTeamDetail { get; set; }=new WhereTeamDetail();
    }
}
