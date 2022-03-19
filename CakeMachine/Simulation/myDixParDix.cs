using System.Runtime.CompilerServices;
using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal class myDixParDix : Algorithme
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
                var plats = new[] { new Plat(), new Plat() , new Plat()};
                var gâteauxCrus = plats
                     // tu appelles le traitement Preparer sur chacun des plats   
                     .AsParallel()
                     .Select(postePréparation.Préparer)
                     .ToArray();

                var gâteauxCuits = posteCuisson.Cuire(gâteauxCrus);

                var gâteauxEmballés = gâteauxCuits
                    .AsParallel()
                    .Select(posteEmballage.Emballer);
                    

                foreach (var gâteauEmballé in gâteauxEmballés)
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
                var plat1 = new Plat();
                var plat2 = new Plat();
                var plat3 = new Plat();
                
                var gâteauCru1Task = postePréparation.PréparerAsync(plat1);
                var gâteauCru2Task = postePréparation.PréparerAsync(plat2);
                var gâteauCru3Task = postePréparation.PréparerAsync(plat3);
                
                var gâteauxCrus = await Task.WhenAll(gâteauCru1Task, gâteauCru2Task,gâteauCru3Task);

                var gâteauxCuits = await posteCuisson.CuireAsync(gâteauxCrus);
                    
                var gâteauEmballé1Task = posteEmballage.EmballerAsync(gâteauxCuits[0]);
                var gâteauEmballé2Task = posteEmballage.EmballerAsync(gâteauxCuits[1]);
                var gâteauEmballé3Task = posteEmballage.EmballerAsync(gâteauxCuits[2]);

                var terminéeEnPremier = await Task.WhenAny(gâteauEmballé1Task, gâteauEmballé2Task, gâteauEmballé3Task);
                yield return await terminéeEnPremier;

                var terminéeEnDernier =
                    gâteauEmballé1Task == terminéeEnPremier ? gâteauEmballé2Task : gâteauEmballé1Task;

                yield return await terminéeEnDernier;
            }
        }
    }
}