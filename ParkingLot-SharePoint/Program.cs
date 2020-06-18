using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Sharing;
using System.Collections.Generic;
using ParkingLot_SharePoint.Models;
using ParkingLot_SharePoint.Services.Services;
using System;
using System.Runtime.Remoting.Contexts;
using User = ParkingLot_SharePoint.Models.User;

namespace ParkingLot
{
    class Program
    {
        static void Main()
        {
            ClientContext clientContext = ParkingLot_SharePoint.Services.SharePointService.CommonService.GetonlineContext();
            UserService userService = new UserService(clientContext);
            while (true)
            {
                try
                {
                    UserService.CurrentUser = userService.Login(Login());
                    if (UserService.CurrentUser != null)
                    {
                        if (UserService.CurrentUser.FirstTimeLogin)
                        {
                            userService.FirstTimeLogin(UserService.CurrentUser.Id, RePassword());
                        }
                        if (UserService.CurrentUser.Type == Convert.ToString(Role.Admin))
                        {
                            ParkingLotService parkingLotService = new ParkingLotService(clientContext);
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine("1. Add new Member\n2. Add new parking lots\n3. Add other parking in lot\n4. Modify parking slot\n5. Logout\n6. Exit");
                                Int32.TryParse(Console.ReadLine(), out int option);
                                switch ((ManagerMenu)option)
                                {
                                    case ManagerMenu.
                                        userService.AddNewUser(AddNewUser(userService, parkingLotService));
                                        break;

                                    case 2:
                                        if (parkingLotService.AddNewParkingLot(AddNewParkingLots()))
                                        {
                                            var firstSetupDetails = SetupParkingConfiguration(parkingLotService);
                                            if (firstSetupDetails != null)
                                                parkingLotService.SetupParkingLot(firstSetupDetails);
                                            break;
                                        }
                                        break;

                                    case 3:
                                        var setupDetails = SetupParkingConfiguration(parkingLotService);
                                        if (setupDetails != null)
                                            parkingLotService.SetupParkingLot(setupDetails);
                                        break;

                                    case 4:
                                        var data = ModifyConfiguration(parkingLotService);
                                        if (data != null)
                                        {
                                            parkingLotService.ModifyParkingLotCongiguration(data);
                                        }
                                        break;

                                    case 5:

                                        break;

                                    case 6:
                                        Environment.Exit(0);
                                        break;

                                    default:

                                        break;
                                }
                                if (option == 5)
                                    break;

                                Console.ReadKey();
                            }
                        }

                        else if (UserService.CurrentUser.Type == Convert.ToString(Role.Manager))
                        {
                            ParkingService parkingService = new ParkingService(clientContext);
                            while (true)
                            {
                                Console.Clear();
                                Console.Write("1. Parking new vehical\n2. Release vehical\n3. View all parked vehical\n4. Logout\n5. Exit");
                                Int32.TryParse(Console.ReadLine(), out int choice);
                                switch (choice)
                                {
                                    case 1:
                                        var parking = ParkVehical(parkingService);
                                        if (parking != null)
                                            parkingService.ParkVehical(parking);
                                        break;


                                    case 2:
                                        int id = ReleaseVehical(parkingService);
                                        if (id == 0)
                                        {
                                            Console.WriteLine("This vehical is not parked in your parking");
                                        }
                                        else if (id == -1)
                                        {
                                            Console.WriteLine("This vehical is releases previously");
                                        }
                                        else
                                        {
                                            var fare = parkingService.ReleaseVehical(id);
                                            Console.WriteLine("Total fare is " + fare + "RS" + "\n" + "Amount has been collected by " + UserService.CurrentUser.Name);
                                        }

                                        break;
 
                                    case 3:
                                        AllParkedVehicle(parkingService.AllParkedVehical());
                                        break;

                                    case 4:
                                        break;
                                    case 5:
                                        Environment.Exit(0);
                                        break;

                                    default:
                                        break;

                                }
                                if (choice == 4)
                                    break;

                                Console.ReadKey();
                            }
                        }

                        else
                        {
                            Console.WriteLine("Please contact to your admin");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter correct username or password");
                    }
                }
                catch (System.Net.WebException)
                {
                    Console.WriteLine("Your internet connection is not connected please retry");
                }
                catch (ServerException)
                {
                    Console.WriteLine("Server error");
                }
            }
        }

        public static string RePassword()
        {
            Console.Write("New Password : ");
            return Console.ReadLine();
        }

        public static Login Login()
        {
            Login login = new Login();
            Console.WriteLine("Please enter credentials");
            Console.Write("UserName : ");
            login.UserName = Console.ReadLine();
            Console.Write("Password : ");
            login.Password = Console.ReadLine();

            return login;
        }

        public static User AddNewUser(UserService userService, ParkingLotService service)
        {
            User user = new User();
            Console.WriteLine("Please enter user's details");
            Console.Write("Name : ");
            user.Name = Console.ReadLine();
            while (true)
            {
                Console.Write("UserName : ");
                var userName = Console.ReadLine();
                if (userService.HasUserName(userName))
                {
                    user.UserName = userName;
                    break;
                }
                else
                {
                    Console.WriteLine("This username is assigned to someone please choose another ");
                }
            }
            Console.Write("Password : ");
            user.Password = Console.ReadLine();
            Console.Write("Address : ");
            user.Address = Console.ReadLine();
            Console.WriteLine("User type for :\n               1. Admin\n               2. Manager");
            while (true)
            {
                Int32.TryParse(Console.ReadLine(), out int type);
                if (type == 1 || type == 2)
                {
                    if (type == 1)
                        user.Type =Convert.ToString(Role.Admin);
                    else if (type == 2)
                    {
                        user.Type = Convert.ToString(Role.Manager);
                        while (true)
                        {
                            Console.Write("Parking slot assign : ");
                            var id = service.HasParkingLot(Console.ReadLine());
                            if (id != 0)
                            {
                                user.ParkingLot = id;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Please enter correct parking lot name");
                            }
                        }
                    }
                    break;
                }
            }

            return user;
        }

        public static ParkingLotInfo SetupParkingConfiguration(ParkingLotService service)
        {
            ParkingLotInfo parkingLot = new ParkingLotInfo();
            Console.WriteLine("Enter new details in parking");
            Console.Write("Parking Name : ");
            var id = service.HasParkingLot(Console.ReadLine());
            if (id != 0)
            {
                parkingLot.ParkingLotId = id;
                Console.Write("Vehical type : ");
                parkingLot.VehicalType= Console.ReadLine();
                while (true)
                {
                    Console.Write("Space : ");
                    Int32.TryParse(Console.ReadLine(), out int space);
                    if (space > 0)
                    {
                        parkingLot.Space = space;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter correct space");
                    }
                }
                while (true)
                {
                    Console.Write("Rate/Hr. : ");
                    Int32.TryParse(Console.ReadLine(), out int rate);
                    if (rate > 0)
                    {
                        parkingLot.Rate = rate;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter correct rate");
                    }
                }
                return parkingLot;
            }
            else
            {
                Console.WriteLine("This parking lot is not available");
                return null;
            }
        }

        public static ParkingLotInfo ModifyConfiguration(ParkingLotService service)
        {
            ParkingLotInfo parkingLot = new ParkingLotInfo();
            Console.WriteLine("Which record you wants to modify");
            Console.Write("Name of the parking lot : ");
            var id = service.HasParkingLot(Console.ReadLine());
            if (id != 0)
            {
                parkingLot.ParkingLotId = id;
                Console.Write("Enter vehical type : ");
                var parkingLotInfoId=service.GetParkingLotId(Console.ReadLine());
                if (parkingLotInfoId != 0)
                {
                    parkingLot.Id = parkingLotInfoId;
                    Console.Write("Rate : ");
                    Int32.TryParse(Console.ReadLine(), out int rate);
                    parkingLot.Rate = rate;
                    Console.Write("Space : ");
                    Int32.TryParse(Console.ReadLine(), out int space);
                    parkingLot.Space = space;
                    return parkingLot;
                }
            }

            return null;
        }

        public static ParkingLotsInfo AddNewParkingLots()
        {
            ParkingLotsInfo parkingLot = new ParkingLotsInfo();
            Console.WriteLine("Enter parking lot details");
            Console.Write("Name : ");
            parkingLot.Name = Console.ReadLine();
            Console.Write("Address:");
            parkingLot.Address = Console.ReadLine();
            return parkingLot;
        }

        public static ParkedVehical ParkVehical(ParkingService parkingService)
        {
            ParkedVehical vehical = new ParkedVehical();
            Console.WriteLine("Please enter details");
            Console.Write("Vehical Type : ");
            var type = parkingService.ParkingAvailable(Console.ReadLine());
            if (type >= 1)
            {
                Console.Write("Vehical number : ");
                vehical.VehicalNumber = Console.ReadLine();

                if (parkingService.ParkingStatus(vehical.VehicalNumber) == null)
                {
                    vehical.ParkingLot = type;
                    return vehical;
                }
                else
                {
                    Console.WriteLine(vehical.VehicalNumber + " already parked in parking");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("For this vehical parking is not available");
                return null;
            }
        }

        public static int ReleaseVehical(ParkingService parkingService)
        {
            ParkedVehical vehical = new ParkedVehical();
            Console.Write("Please enter vehical number : ");
            vehical.VehicalNumber = Console.ReadLine();
            int id= parkingService.GetVehicalId(vehical.VehicalNumber);
            return id; 
        }

        public static void AllParkedVehicle(List<ParkedVehical> vehicles)
        {
            if (vehicles.Count > 0)
            {
                Console.WriteLine("Vehicle Number\tEntry Time");
                foreach (var vehicle in vehicles)
                {
                    Console.WriteLine(vehicle.VehicalNumber + "\t" + vehicle.EnteyTime);
                }
            }
            else
            {
                Console.WriteLine("No vehicle parked right now.");
            }

            Console.ReadKey();
        }
    }
}

public enum Role
{
    Admin,
    Manager
}

public enum ManagerMenu
{

}

public enum AdminMenu
{
    AddUser=1,
    AddParkingLot, 
    SetupParkingConfiguration,
    ModifyParkingConfiguration,
    Logout,
    Exit
}