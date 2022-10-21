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


            foreach (FamilyInstance element in listOfAllBeams)
            {

                // Creates the TypeId
                var cast = (Element)element;
                int typeID = cast.Id.IntegerValue;

                //Maps the material of the beam
                string materialID = element.StructuralMaterialType.ToString();


                //Maps the length of the beam
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);


                ////Maps the crossSectionArea
                // I cant find the rigth method to extract the crosssectionarea.
                //var crossSectionArea = element.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_AREA).AsDouble();
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);
                double crossSectionArea = volume / length;

                Beams beams = new Beams(typeID, materialID, length, crossSectionArea);
            }

            foreach (FamilyInstance element in listOfAllColumns)
            {
                // Creates the TypeId
                var cast = (Element)element;
                int typeID = cast.Id.IntegerValue;

                //Maps the material of the beam
                string materialID = element.StructuralMaterialType.ToString();


                //Maps the length of the column
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM).AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);

                ////Maps the crossSectionArea
                // I cant find the right method to extract the crosssectionarea.
                //var crossSectionArea = element.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_AREA).AsDouble();
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);
                double crossSectionArea = volume / length;

                Columns column = new Columns(typeID, materialID, length, crossSectionArea);
            }

            return Result.Succeeded;
            



        }


    }
}


    


