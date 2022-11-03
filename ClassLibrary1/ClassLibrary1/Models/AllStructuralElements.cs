using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models
{
    public class AllStructuralElements
    {
        //public Dictionary<string, Dictionary<string, StructuralElement>> structuralElement { get; set; } = new Dictionary<string, structuralElement>();


        public AllStructuralElements()
        {

        }

    //    public void AddComponent(Component component)
    //    {
    //        if (!SubSystems.ContainsKey(component.SystemType))
    //        {
    //            CreateSubSystem(component.SystemType);
    //        }

    //        SubSystems[component.SystemType].AddComponent(component);
    //    }

    //    public void CreateSubSystem(string type)
    //    {
    //        SubSystem subSystem = new SubSystem(type);
    //        SubSystems[type] = subSystem;
    //    }

    //    public void BuildLocation(string systemName)
    //    {
    //        throw new NotImplementedException();
    //    }


    //}
    //public class SubSystem
    //{
    //    public string Type { get; set; }
    //    public List<Component> Components { get; set; } = new List<Component>();

    //    public SubSystem(string type)
    //    {
    //        Type = type;
    //    }

    //    public void AddComponent(Component component)
    //    {
    //        Components.Add(component);
    //    }
    }
}
