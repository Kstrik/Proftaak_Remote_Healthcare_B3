using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SimulatieTest
{
    public class SimulationBike : Bike
    {
        private List<PageSimulation> pageSimulations;

        private bool isSimulating;

        public SimulationBike(IBikeDataReceiver bikeDataReceiver) 
            : base(bikeDataReceiver)
        {
            this.pageSimulations = new List<PageSimulation>();
            this.isSimulating = false;
        }

        public override void ReceivedData(byte[] data)
        {
            base.ReceivedData(data);
        }

        public void AddPageSimualtion(Page page, double variance, double simulationInterval)
        {
            if(page != null && simulationInterval != 0)
            {
                PageSimulation pageSimulation = new PageSimulation(page, variance, this, simulationInterval);
                this.pageSimulations.Add(pageSimulation);

                if(this.isSimulating)
                {
                    pageSimulation.StartSimulating();
                }
            }
        }

        public override bool ToggleListening()
        {
            if (this.pageSimulations.Count == 0)
                return false;

            this.isSimulating = !this.isSimulating;
            foreach(PageSimulation pageSimulation in this.pageSimulations)
            {
                pageSimulation.ToggleSimulation();
            }
            return true;
        }

        public override bool StartListening()
        {
            if (this.pageSimulations.Count == 0)
                return false;

            this.isSimulating = true;
            foreach (PageSimulation pageSimulation in this.pageSimulations)
            {
                pageSimulation.StartSimulating();
            }
            return true;
        }

        public override bool StopListening()
        {
            if (this.pageSimulations.Count == 0)
                return false;

            this.isSimulating = false;
            foreach (PageSimulation pageSimulation in this.pageSimulations)
            {
                pageSimulation.StopSimulating();
            }
            return true;
        }
    }
}
