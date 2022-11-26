using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Forms;
using Autodesk.Revit.Creation;
using System.Xml.Linq;
using Document = Autodesk.Revit.DB.Document;
using Application = Autodesk.Revit.ApplicationServices.Application;
using StructuralElementsExporter.Models;
using StructuralElementsExporter.Helpers;
using Autodesk.Revit.DB.Structure.StructuralSections;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Autodesk.Revit.DB.Visual;
using System.Globalization;
using System.IO;
using Autodesk.Revit.DB.Structure;
using Newtonsoft.Json;
using StructuralElementsExporter.Models.Containers;
using System.Runtime.CompilerServices;

namespace StructuralElementsExporter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SelectElements : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector wall_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allWalls = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            List<Wall> listOfAllWalls = wall_collector.WherePasses(allWalls).WhereElementIsNotElementType().Cast<Wall>().ToList();
            List<Wall> listOfAllExteriorWalls = new List<Wall>();
            List<Wall> listOfAllInteriorWalls = new List<Wall>();

            FilteredElementCollector beam_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allBeams = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            List<FamilyInstance> listOfAllBeams = beam_collector.WherePasses(allBeams).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            FilteredElementCollector column_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allColumns = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            List<FamilyInstance> listOfAllColumns = column_collector.WherePasses(allColumns).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            FilteredElementCollector deck_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allDecks = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            List<Floor> listOfAllDecks = deck_collector.WherePasses(allDecks).WhereElementIsNotElementType().Cast<Floor>().ToList();

            FilteredElementCollector foundation_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allFoundation = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            List<Element> listOfAllFoundation = foundation_collector.WherePasses(allFoundation).WhereElementIsNotElementType().Cast<Element>().ToList();



            // Creates a Lists of all exterior and interior walls 
            foreach (Wall element in listOfAllWalls)
            {
                var test = element.WallType.Function;
                if (test == WallFunction.Exterior)
                {
                    listOfAllExteriorWalls.Add(element);
                }
                else if (test == WallFunction.Interior)
                {
                    listOfAllInteriorWalls.Add(element);
                }
            }

            ExteriorWalls exteriorWalls = new ExteriorWalls();
            // Assigns the revit parameters to the Outerwall constructor
            foreach (Element element in listOfAllExteriorWalls)
            {
                var structuralUsage = element.LookupParameter("Structural Usage").AsValueString();

                if (structuralUsage == "Bearing")
                {
                    // Creates the TypeId
                    WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                    // Change from var to int
                    int typeID = walltype.Id.IntegerValue;

                    //Creates Structural material
                    string materialID = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                    // Maps the area of the wall
                    double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                    double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                    // Maps the thickness of the wall
                    WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                    double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(wallType.Width);
                    double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 2);


                    ExteriorWall exteriorWall = new ExteriorWall(typeID, materialID, area, thickness);
                    exteriorWalls.AddExteriorWall(exteriorWall);

                }
            }

            InteriorWalls interiorWalls = new InteriorWalls();
            // Assigns the revit parameters to the Innerwall constructor
            foreach (Element element in listOfAllInteriorWalls)
            {
                var structuralUsage = element.LookupParameter("Structural Usage").AsValueString();

                if (structuralUsage == "Bearing")
                {

                    // Creates the TypeId
                    WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                    // Change from var to int
                    int typeID = walltype.Id.IntegerValue;

                    //Creates Structural material
                    string materialID = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                    // Maps the area of the wall
                    double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                    double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                    // Maps the thickness of the wall
                    WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                    double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(wallType.Width);
                    double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 2);

                    InteriorWall interiorWall = new InteriorWall(typeID, materialID, area, thickness);
                    interiorWalls.AddInteriorWall(interiorWall);

                }
            }

            Beams beams = new Beams();
            // Assigns the revit parameters to the Beam constructor
            foreach (FamilyInstance familyInstance in listOfAllBeams)
            {

                // Creates the TypeId
                var cast = (Element)familyInstance;
                int typeID = cast.Id.IntegerValue;


                //Maps the material of the beam
                string material = familyInstance.StructuralMaterialType.ToString();
                string test = Convert.ToString(doc.GetElement(familyInstance.StructuralMaterialId) as Material);
                
                string quality;
                
                if (test == "")
                {
                    quality = "Not defined";

                }
                else
                {
                    var quality1 = doc.GetElement(familyInstance.StructuralMaterialId) as Material;
                    quality = quality1.Name.ToString();

                }
                
                
                //Maps the length of the beam
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(familyInstance.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                //Alternative way to get length
                //var lengthprøve = ImperialToMetricConverter.ConvertFromFeetToMeters(cast.LookupParameter("Length").AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);


                ////Maps the crossSectionArea based on the volume and the length
                ///
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                //Alternative way to get volume
                //var volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(cast.LookupParameter("Volume").AsDouble());
                double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);

                double weight = 0;

                //if (material == "Steel" || material == "steel")
                //{

                //    double weight2 = familyInstance.GetParameters("G");
                   


                //    //var prøveprøve = doc.GetElement(cast.LookupParameter("")


                //    weight = 100000 * weight2;   

                //}
                //else
                //{
                //    weight = 0;

                //}

                //double weight = 0;
                


                



                //string weight2 = doc.GetElement(familyInstance.GetTypeId()).LookupParameter("A").AsValueString();
                //string[] weight3 = weight2.Split(' ');
                //double weight = Double.Parse(weight3[0]) * length;



                Beam beam = new Beam(typeID, material, quality, length, volume, weight);
                beams.AddBeam(beam);


            }

            Columns columns = new Columns();
            // Assigns the revit parameters to the Column constructor
            foreach (FamilyInstance familyInstance in listOfAllColumns)
            {
                // Creates the TypeId
                var cast = (Element)familyInstance;
                int typeID = cast.Id.IntegerValue;

                //Maps the material of the beam
                string materialID = familyInstance.StructuralMaterialType.ToString();


                //Maps the length of the column
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(familyInstance.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM).AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);

                ////Maps the crossSectionArea based on volume and length
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);
                

                Column column = new Column(typeID, materialID, length, volume);
                columns.AddColumn(column);

            }

            Decks decks = new Decks();
            // Assigns the revit parameters to the Deck constructor
            foreach (Element element in listOfAllDecks)
            {
                // Creates the TypeId
                int typeID = element.Id.IntegerValue;

                //Hvordan får man Structural material??
                var carsten = (Floor)element;
                string materialID = carsten.FloorType.LookupParameter("Structural Material").AsValueString();


                // Maps the area of the deck
                double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                // Maps the thickness of the wall
                double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble());
                double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 3);


                Deck deck = new Deck(typeID, materialID, area, thickness);
                decks.AddDeck(deck);


            }

            Foundations foundations = new Foundations();
            // Assigns the revit parameters to the Foundation constructor
            foreach (Element element in listOfAllFoundation)
            {
                var test = element.GetType().Name;
                if ((test == "WallFoundation") || (test == "Floor"))
                {
                    
                    int typeID = element.Id.IntegerValue;

                    string materialID = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                    double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                    double volume = RoundToSignificantDigits.RoundDigits(volume1, 4);

                    Foundation foundation = new Foundation(typeID, materialID, volume);
                    foundations.AddFoundation(foundation);

                }
                else if (test == "FamilyInstance")
                {

                    int typeID = element.Id.IntegerValue;

                    var cast = (FamilyInstance)element;
                    string materialID = cast.LookupParameter("Structural Material").AsValueString();

                    double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                    double volume = RoundToSignificantDigits.RoundDigits(volume1, 4);

                    Foundation foundation = new Foundation(typeID, materialID, volume);
                    foundations.AddFoundation(foundation);
                }
                

            }

            // Add all structural elements to a Dictionary of Structuralelements
            Dictionary<string, List<object>> structuralElements = new Dictionary<string, List<object>>();

            structuralElements.Add("Beam", beams.BeamsInModel);
            structuralElements.Add("Column", columns.ColumnsInModel);
            structuralElements.Add("Deck", decks.DecksInModel);
            structuralElements.Add("ExteriorWall", exteriorWalls.ExteriorWallsInModel);
            structuralElements.Add("InteriorWall", interiorWalls.InteriorWallsInModel);
            structuralElements.Add("Foundation", foundations.FoundationsInModel);


            // Lav breakpoint og kopier JSON filen.
            JsonConvert.SerializeObject(structuralElements);

            File.WriteAllText(@"C:\Users\camil\Documents\Structuralelements_Json", JsonConvert.SerializeObject(structuralElements));

            return Result.Succeeded;

            }

        private static void NaterialID()
        {

        }


    }
    }



    


