using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using NodeNetwork.ViewModels;

namespace ExampleHelloWorld
{
    public class MainWindowVm
    {
        public NetworkViewModel Network { get; set; }

        public MainWindowVm()
        {
            //Create a new viewmodel for the NetworkView
            NetworkViewModel network = new NetworkViewModel();

            //Create the node for the first node, set its name and add it to the network.
            NodeViewModel node1 = new NodeViewModel();
            {
                node1.Name = "Node 1";

                //Create the viewmodel for the input on the first node, set its name and add it to the node.
                NodeInputViewModel node1Input = new NodeInputViewModel();
                node1Input.Name = "Node 1 input";
                node1.Inputs.Add(node1Input);
            }


            //Create the second node viewmodel, set its name, add it to the network and add an output in a similar fashion.
            NodeViewModel node2 = new NodeViewModel();
            {
                node2.Name = "Node 2";

                NodeOutputViewModel node2Output = new NodeOutputViewModel();
                node2Output.Name = "Node 2 output";
                node2.Outputs.Add(node2Output);
            }

            network.Nodes.Add(node1);
            network.Nodes.Add(node2);

            //Assign the viewmodel to the view.
            Network = network;
        }
    }
}
