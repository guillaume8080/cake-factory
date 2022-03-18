using System.Runtime.CompilerServices;
using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal class FourRempli : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => false;
        public override bool SupportsAsync => true;

        private List<Task<GâteauCru>> listOfGateauCrus = new();
        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            var postePréparation = usine.Préparateurs.Single();
            var posteCuisson = usine.Fours.Single();
            var posteEmballage = usine.Emballeuses.Single();

            while (!token.IsCancellationRequested)
            {
                var plats = Enumerable.Range(0, usine.OrganisationUsine.ParamètresCuisson.NombrePlaces)
                    .Select(_ => new Plat());

                var gâteauxCrus = plats.Select(postePréparation.Préparer);
                var gâteauxCuits = posteCuisson.Cuire(gâteauxCrus.ToArray());
                var gâteauxEmballés = gâteauxCuits.Select(posteEmballage.Emballer).ToArray();

                foreach (var gâteauEmballé in gâteauxEmballés)
                    yield return gâteauEmballé;
            }
        }
        public override async IAsyncEnumerable<GâteauEmballé> ProduireAsync(Usine usine, [EnumeratorCancellation] CancellationToken token)
         {
             var postePréparation = usine.Préparateurs.Single();
             var posteCuisson = usine.Fours.Single();
             var posteEmballage = usine.Emballeuses.Single();

             List<Task<GâteauCru>> malistdegateaucru = new();
             
 
             while (!token.IsCancellationRequested)
             {
                 var plats = Enumerable.Range(0, usine.OrganisationUsine.ParamètresCuisson.NombrePlaces)
                     .Select(_ => new Plat());
                 var arrayOfTasksOfPrepaGateauCru =  plats
                     .Select(_ => postePréparation.PréparerAsync(_))
                     //.AsParallel()
                     .ToArray();
                     var x = /*await impossible*/  arrayOfTasksOfPrepaGateauCru;
                 
                 //private GâteauCuit[] arrayOfGateauCuits = new GâteauCuit[0] {};
                 
                 var tabGateauxCuits = await Task.WhenAll(arrayOfTasksOfPrepaGateauCru);
                 
                 var gâteauxCuits = await posteCuisson.CuireAsync(tabGateauxCuits);

                 // var somDrilledCakes = gâteauxCuits.Take(2);
                 // var someDrilledCakes2 = gâteauxCuits.Chunk(2);
                
                 

                 var ArrayOfTaskEmabaleToMake = gâteauxCuits.Select(_ => (posteEmballage.EmballerAsync(_))).ToArray();
                 
                 // GâteauEmballé[] arrayToREcupGateauEmballeAsync 
                 
                
                 
                     var cakeDone = await Task.WhenAny(ArrayOfTaskEmabaleToMake);
                     var z = cakeDone.IsCompleted;
                     var cakeDoneX = cakeDone.Result;
                    
                     //      ArraySegment<string> myArrSegMid = new ArraySegment<string>( myArr, 2, 5 );
                     
                 
                 
                 
                 
             }
             
             var platx = new Plat();

             var gâteauCru = await postePréparation.PréparerAsync(platx);
             var gâteauCuit = (await posteCuisson.CuireAsync(gâteauCru)).Single();
             var gâteauEmballé = await posteEmballage.EmballerAsync(gâteauCuit);

             yield return gâteauEmballé;
 
         }
    }

}