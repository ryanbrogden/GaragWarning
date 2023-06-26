using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        MyIni _ini = new MyIni();
        private string tag;

        private List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
        private IMyShipConnector shipConnector;

        private void SetLightsRed()
        { 
            lights.ForEach(light => {
                light.Color = new Color(255, 0, 0);
            });
        }

        private void SetLightsYellow()
        {
            lights.ForEach(light => {
                light.Color = new Color(255, 255, 0);
            });
        }

        private void SetLightsGreen()
        {
            lights.ForEach(light => {
                light.Color = new Color(0, 255, 0);
            });
        }

        private void SetLightsFlashing()
        {
            lights.ForEach(light => {
                light.BlinkIntervalSeconds = 2;
                light.BlinkLength = 75;
            });
        }

        private void SetLightsSolid()
        {
            lights.ForEach(light => {
                light.BlinkIntervalSeconds = 0;
            });
        }

        private void SetLights(string tag)
        {
            
            GridTerminalSystem.GetBlocksOfType(lights, light => light.DisplayNameText.Contains(tag));

            if (lights.Count < 1)
            {
                Echo($"No lights found with tag: {tag}");
            }
        }

        private void SetShipConnector(string tag)
        {
            List<IMyShipConnector> taggedConnectors = new List<IMyShipConnector>();
            GridTerminalSystem.GetBlocksOfType(taggedConnectors, connector => connector.DisplayNameText.Contains(tag));

            if (taggedConnectors.Count > 0)
            {
                shipConnector = taggedConnectors[0];
            }
            else
            {
                Echo($"No ship connector found with tag {tag}");
            }
        }

        private void ControlLightState()
        {
            if (shipConnector != null && lights.Count > 0)
            {
                if (shipConnector.Status == MyShipConnectorStatus.Connected)
                {
                    Echo("Lights solid green");
                    SetLightsGreen();
                    SetLightsSolid();
                }
                else if (shipConnector.Status == MyShipConnectorStatus.Connectable)
                {
                    Echo("Lights solid yellow");
                    SetLightsYellow();
                    SetLightsSolid();
                }
                else if (shipConnector.Status == MyShipConnectorStatus.Unconnected)
                {
                    Echo("Lights flashing red");
                    SetLightsRed();
                    SetLightsFlashing();
                }
            }
        }

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            MyIniParseResult parseResult;
            if (!_ini.TryParse(Me.CustomData, out parseResult))
            {
                throw new Exception(parseResult.ToString());
            }
            else
            {
                tag = _ini.Get("Garage", "tag").ToString();
            }
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.
            if (tag != null)
            {
                Echo($"tag: {tag}");

                SetLights(tag);
                SetShipConnector(tag);
            }
            else
            {
                Echo("No tag found");
            }

            ControlLightState();
        }
    }
}
