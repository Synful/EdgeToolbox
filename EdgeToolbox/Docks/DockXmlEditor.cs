using DarkUI.Collections;
using DarkUI.Config;
using DarkUI.Controls;
using DarkUI.Docking;
using DarkUI.Forms;
using EdgeDeviceLibrary.Communicator;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace EdgeToolbox {
    public partial class DockXmlEditor : DarkDocument {
        XmlDocument _doc = new XmlDocument();
        Proteus p;

        public DockXmlEditor(string dir, string file) {
            InitializeComponent();

            p = Proteus.Instance();

            this.DockText = file;

            RenderXMLFile(dir, file);
            //foreach (XmlNode xNode in _doc.DocumentElement.ChildNodes) {
            //    var node = new DarkTreeNode($"{xNode.Name}");
            //    if (xNode.HasChildNodes) {
            //        foreach(XmlAttribute attribute in xNode.Attributes) {
            //            var child = new DarkTreeNode($"{attribute.Name} | {attribute.Value}");
            //            node.Nodes.Add(child);
            //        }
            //    }
            //    xmlView.Nodes.Add(node);
            //}
        }

        private void RenderXMLFile(string dir, string file) {
            try {
                MemoryStream ms = new MemoryStream();

                p.ChangeDirectory($"{dir}");
                p.ReadFat($"{file}", false, false, ref ms);

                XmlReader reader = new XmlTextReader(ms);
                _doc.Load(reader);

                // 3. Initialize the TreeView control. treeView1 can be created dinamically
                // and attached to the form or you can just drag and drop the widget from the toolbox
                // into the Form.

                // Clear any previous content of the widget
                xmlView.Nodes.Clear();
                // Create the root tree node, on any XML file the container (first root)
                // will be the DocumentElement name as any content must be wrapped in some node first.
                xmlView.Nodes.Add(new DarkTreeNode(_doc.DocumentElement.Name));

                // 4. Create an instance of the first node in the treeview (the one that contains the DocumentElement name)
                DarkTreeNode tNode = new DarkTreeNode();
                tNode = xmlView.Nodes[0];

                // 5. Populate the TreeView with the DOM nodes.
                this.AddNode(_doc.DocumentElement, tNode);
            } catch (XmlException xmlEx) {
                MessageBox.Show(xmlEx.Message);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Renders a node of XML into a TreeNode. Recursive if inside the node there are more child nodes.
        /// </summary>
        /// <param name="inXmlNode"></param>
        /// <param name="inTreeNode"></param>
        private void AddNode(XmlNode inXmlNode, DarkTreeNode inTreeNode) {
            XmlNode xNode;
            DarkTreeNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.HasChildNodes) {
                nodeList = inXmlNode.ChildNodes;

                for (i = 0; i <= nodeList.Count - 1; i++) {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new DarkTreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    this.AddNode(xNode, tNode);
                }
            } else {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        public override void Close() {
            var result = DarkMessageBox.ShowWarning(null, @"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
            if(result == DialogResult.No)
                return;

            base.Close();
        }
    }
}
