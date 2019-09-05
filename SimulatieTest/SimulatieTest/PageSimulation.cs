using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SimulatieTest
{
    public class PageSimulation
    {
        private Page page;
        private SimulationBike simulationBike;
        private Timer timer;

        private double varience;
        protected Random random;

        private bool isSimulating;

        public PageSimulation(Page page, double varience, SimulationBike simulationBike, double simulationInterval)
        {
            this.page = page;
            this.simulationBike = simulationBike;
            this.timer = new Timer(simulationInterval);
            this.timer.Elapsed += Timer_Elapsed;

            this.varience = varience;
            this.random = new Random();
        }

        public void ToggleSimulation()
        {
            this.isSimulating = !this.isSimulating;
            CheckTimer();
        }

        public void StartSimulating()
        {
            this.isSimulating = true;
            CheckTimer();
        }

        public void StopSimulating()
        {
            this.isSimulating = false;
            CheckTimer();
        }

        private void CheckTimer()
        {
            if (this.isSimulating && !this.timer.Enabled)
            {
                this.timer.Start();
            }
            else if (!this.isSimulating && this.timer.Enabled)
            {
                this.timer.Stop();
            }
        }

        private void Simulate()
        {
            Page simulatedPage = this.page.SimulateNewPage(this.varience, this.random);
            BikeMessage message = new BikeMessage(simulatedPage);
            simulationBike.ReceivedData(message.GetBytes());
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Simulate();
        }
    }
}
