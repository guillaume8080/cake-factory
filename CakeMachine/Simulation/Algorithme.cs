using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal abstract class Algorithme
    {
        public virtual bool SupportsAsync => false;
        public virtual bool SupportsSync => false;

        public virtual void ConfigurerUsine(IConfigurationUsine builder)
        {
            
        }

        public virtual IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            // stocker 2 gateaux dans l enumerable
            // yield return usine.Préparateurs.Préparer();
            
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<GâteauEmballé> ProduireAsync(Usine usine, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
