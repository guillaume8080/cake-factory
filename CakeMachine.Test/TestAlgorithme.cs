using System;
using System.Linq;
using System.Threading;
using CakeMachine.Fabrication;
using CakeMachine.Simulation;
using Xunit;

namespace CakeMachine.Test
{
    public class TestAlgorithme
    {
        [Fact]
        public void TestSingleThread()
        {
            var singleThread = new SingleThread();
            var usine = new UsineBuilder().Build();

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            singleThread.Produire(usine, cancellationTokenSource.Token).ToArray();
        }
        
        
        [Fact]
        public void TestDixParDix()
        {
            var algo = new DixParDix();
            var usine = new UsineBuilder().Build();

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            algo.Produire(usine, cancellationTokenSource.Token).ToArray();
        }

        [Fact]
        public async void TestDixParDixAsync()
        {
            var algo = new DixParDix();
            var usine = new UsineBuilder().Build();

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            // on boucle ici car la methode DixParDix n'est pas appellee une seukle fois mais autant qu il faut pour 100 gateaux
            await foreach (var _ in algo.ProduireAsync(usine, cancellationTokenSource.Token))
            {
                
            }

        }

        [Fact]
        public void TestFourRempli()
        {
            var algo = new FourRempli();
            var usine = new UsineBuilder().Build();
            
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            algo.Produire(usine, cancellationTokenSource.Token).ToArray();
            
            
        }

        [Fact]

        public async void FourRempliAsyc()
        {
            var algo = new FourRempli();
            var usine = new UsineBuilder().Build();
            
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await foreach (var _ in algo.ProduireAsync(usine, cancellationTokenSource.Token))
            {
                
            }
        }

    }
}