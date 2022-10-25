﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Forms;
using Autodesk.Revit.Creation;
using System.Xml.Linq;
using Document = Autodesk.Revit.DB.Document;
using Application = Autodesk.Revit.ApplicationServices.Application;
using ClassLibrary1.Models;
using ClassLibrary1.Helpers;
using Autodesk.Revit.DB.Structure.StructuralSections;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Autodesk.Revit.DB.Visual;



namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SelectElements : IExternalCommand
    {

        //Find parameter using the Parameter's definition type.


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;


            
            //Building a string that contains all the structural elements (Container)
            //StringBuilder structuralElements = new StringBuilder();
            //AllStructuralElements allStructuralelements = new AllStructuralElements();

            //var allElements = HelperFunctions.GetConnectorElements(doc);
            //var allApaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement));

            //structuralElements = Mapper.MapAllComponents(allElements);

            

            FilteredElementCollector collection = new FilteredElementCollector(doc);
            ElementCategoryFilter allWalls = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            List<Wall> listOfAllWalls = collection.WherePasses(allWalls).WhereElementIsNotElementType().Cast<Wall>().ToList();
            List<Wall> exteriorWalls = new List<Wall>();
            List<Wall> interiorWalls = new List<Wall>();

            FilteredElementCollector beam_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allBeams = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            List<FamilyInstance> listOfAllBeams = beam_collector.WherePasses(allBeams).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();
            
            FilteredElementCollector column_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allColumns = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            List<FamilyInstance> listOfAllColumns = column_collector.WherePasses(allColumns).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            FilteredElementCollector deck_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allDecks = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            List<Floor> listOfAllDecks = deck_collector.WherePasses(allDecks).WhereElementIsNotElementType().Cast<Floor>().ToList();

            // Creates a Lists of all exterior and interior walls 
            foreach (Wall element in listOfAllWalls)
            {
                var test = element.WallType.Function;
                if (test == WallFunction.Exterior)
                {
                    exteriorWalls.Add(element);
                }
                else if (test == WallFunction.Interior)
                {
                    interiorWalls.Add(element);
                }
            }

            // Assigns the revit parameters to the Outerwall constructor
            foreach (Element element in exteriorWalls)
            {
                // Creates the TypeId
                WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                // Change from var to int
                int typeID = walltype.Id.IntegerValue;

                //CompoundStructureLayer compoundStructureLayer = 
                //string MaterialID = compoundStructureLayer.MaterialId

                //Hvordan får man Structural material??
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;
                //var materialID = familyInstance.StructuralMaterialType;              
                string materialID = "Concrete";
                //WallType.CompoundStructure.Layers
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;


                //WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                
                

                // Maps the area of the wall
                double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                // Maps the thickness of the wall
                WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(wallType.Width);
                double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 2);


                OuterWall outerWall = new OuterWall(typeID, materialID, area, thickness);

            }

            // Assigns the revit parameters to the Innerwall constructor
            foreach (Element element in interiorWalls)
            {
                // Creates the TypeId
                WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                // Change from var to int
                int typeID = walltype.Id.IntegerValue;

                //Hvordan får man Structural material??
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;
                //var materialID = familyInstance.StructuralMaterialType;              
                string materialID = "Concrete";

                // Maps the area of the wall
                double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                // Maps the thickness of the wall
                WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(wallType.Width);
                double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 2);

                InnerWall innerWall = new InnerWall(typeID, materialID, area, thickness);

            }

            foreach (FamilyInstance familyInstance in listOfAllBeams)
            {

                //double area1 = element.StructuralSection.SectionArea;
                //var area1 = familyInstance.LookupParameter("A");

                


                //var area3 = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_AREA).AsValueString();




                //Parameter parameter2 = familyInstance.LookupParameter("A");
                //parameter2.AsDouble().ToString();



                // Creates the TypeId
                var cast = (Element)familyInstance;
                int typeID = cast.Id.IntegerValue;

                

                //Maps the material of the beam
                string materialID = familyInstance.StructuralMaterialType.ToString();


                //Maps the length of the beam
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(familyInstance.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);


                ////Maps the crossSectionArea
                // I cant find the rigth method to extract the crosssectionarea.
                //var crossSectionArea = element.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_AREA).AsDouble();
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);
                //double crossSectionArea = volume / length;

                //Crosssection area in cm2
                double test = 10.3;
                string crossSectionArea3 = familyInstance.LookupParameter("A").AsValueString();
                string[] crossSectionArea4 = crossSectionArea3.Split(' ');
                double crossSectionArea = Double.Parse(crossSectionArea4[0]);



                Beam beam = new Beam(typeID, materialID, length, crossSectionArea);


            }

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

                ////Maps the crossSectionArea
                // I cant find the right method to extract the crosssectionarea.
                //var crossSectionArea = element.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_AREA).AsDouble();
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);
                double crossSectionArea = volume / length;

                Column column = new Column(typeID, materialID, length, crossSectionArea);
            }

            return Result.Succeeded;

            //MessageBox.Show(sb.ToString());


        }


    }
}


    


