using System.Runtime.CompilerServices;
using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    public class SingleThread : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => true;

        /// <inheritdoc />
        public override bool SupportsAsync => true;

        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            var postePréparation = usine.Préparateurs.Single();
            var posteCuisson = usine.Fours.Single();
            var posteEmballage = usine.Emballeuses.Single();

            while (!token.IsCancellationRequested)
            {
                var plat = new Plat();

                var gâteauCru = postePréparation.Préparer(plat);
                var gâteauCuit = posteCuisson.Cuire(gâteauCru).Single();
                var gâteauEmballé = posteEmballage.Emballer(gâteauCuit);
                
                yield return gâteauEmballé;
            }
        }

        /// <inheritdoc />
        public override async IAsyncEnumerable<GâteauEmballé> ProduireAsync(Usine usine, [EnumeratorCancellation] CancellationToken token)
        {
            var postePréparation = usine.Préparateurs.Single();
            var posteCuisson = usine.Fours.Single();
            var posteEmballage = usine.Emballeuses.Single();

            while (!token.IsCancellationRequested)
            {
                var plat = new Plat();

                var gâteauCru = await postePréparation.PréparerAsync(plat);
                var gâteauCuit = (await posteCuisson.CuireAsync(gâteauCru)).Single();
                var gâteauEmballé = await posteEmballage.EmballerAsync(gâteauCuit);

                yield return gâteauEmballé;
            }
        }
    }
}