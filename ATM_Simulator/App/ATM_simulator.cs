﻿using ATM_Simulator.App.UI;
using ATM_Simulator.Domain.Entities;
using ATM_Simulator.Domain.Enum;
using ATM_Simulator.Domain.Interfaces;

namespace ATM_Simulator;


class ATM_simulator : IUserLogin, IUserAccountActions
{
    private List<UserAccount> userAccountList;
    private UserAccount selectedAccount;

    public void Run()
    {
        AppScreen.Welcome();
        ChcekUserCardNumAndPassword();
        AppScreen.WelcomeCustomer(selectedAccount.FullName);
        AppScreen.DispalyAppMenu();
        ProcessMenuOption(); 
    }


    public void InitializeData()
    {
        userAccountList = new List<UserAccount>
        {
            new UserAccount{Id=1, FullName="Patryk Walendziuk",AccountNumber=123456, CardNumber=321321, CardPin=123123, AccountBalance=50000.00m, IsLocked=false},
            new UserAccount{Id=2, FullName="Barack Obama",AccountNumber=456789, CardNumber=654654, CardPin=456456, AccountBalance=100000.00m, IsLocked=false},
            new UserAccount{Id=3, FullName="Andrzej Duda",AccountNumber=123555, CardNumber=987987, CardPin=789789, AccountBalance=2900000.00m, IsLocked=true},
        };
    }

    public void ChcekUserCardNumAndPassword()
    {
        bool isCorrectLogin = false;

        while (isCorrectLogin == false)
        {
            restart:
            UserAccount inputAccount = AppScreen.UserLoginForm();
            AppScreen.LoginProgress();

            
            foreach (UserAccount account in userAccountList)
            {

                selectedAccount = account;

                //if typed card number is in database
                if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                {
                    selectedAccount.TotalLogin++;

                    if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                    {
                        //selectedAccount = account;

                        if (selectedAccount.IsLocked)
                        {
                            AppScreen.PrintLockScreen();
                            break;
                        }
                        else
                        {
                            selectedAccount.TotalLogin = 0;
                            isCorrectLogin = true;
                            break;
                        }
                    }

                }

                //if login or PIN was incorrect
                if (isCorrectLogin == false)
                {
                    Utility.PrintMessage("\nInvalid card number or PIN.", false);

                    //if user typed correct card number and wrong pin 3 times, account get blocked
                    selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;

                    if (selectedAccount.IsLocked)
                    {
                        AppScreen.PrintLockScreen();
                    }
                }
                Console.Clear();
                goto restart;
            }
        }


    }
    
    private void ProcessMenuOption()
    {
        switch(Validator.Convert<int>("an option:"))
        {
            case (int)AppMenu.CheckBalance:
                CheckBalance();
                break;
            case (int)AppMenu.PlaceDeposit:
                Console.WriteLine("Placing deposit...");
                break;
            case (int)AppMenu.MakeWithdrawl:
                Console.WriteLine("Making withdrawl...");
                break;
            case (int)AppMenu.InterlanTransfer:
                Console.WriteLine("Making internal transfer...");
                break;
            case (int)AppMenu.ViewTransaction:
                Console.WriteLine("Viewing transaction...");
                break;
            case (int)AppMenu.Logout:
                AppScreen.LogoutProgress();
                Utility.PrintMessage("You have successfully logged out. Please collect your ATM card.");
                Run();
                break;
            default:
                Utility.PrintMessage("Invalid option.", false);
                break;
        }
    }

    public void CheckBalance()
    {
        Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}.");
    }

    public void PlaceDeposit()
    {
        Console.WriteLine("Only multiples of 100 and 50 polish zloty allowed");
        var transaction_amount = Validator.Convert<int>($"amount {AppScreen.cur}");

        //simulate checking
        Console.WriteLine("\nChecking and counting bank notes.");
        Utility.PrintDotAnimation();
        Console.WriteLine("");

        //some guard clause
        if(transaction_amount <= 0)
        {
            Utility.PrintMessage("Amount needs to be greater than 0. Try again.", false);
            return;
        }
        if(transaction_amount % 500 != 0)
        {
            Utility.PrintMessage($"Enter deposit amount in multiples of 50 or 100. Try again.");
            return;
        }

        if(PreviewBankZlotysCount(transaction_amount) == false)
        {
            Utility.PrintMessage($"You have cancelled your action.", false);
            return;
        }
    }

    public void MakeWithDrawal()
    {
        throw new NotImplementedException();
    }

    private bool PreviewBankZlotysCount(int amount)
    {
        int hundredsZlotyCount = amount / 100;
        int fiftiesZlotyCount = (amount % 100) / 50;

        Console.WriteLine("\nSummary");
        Console.WriteLine("---------");
        Console.WriteLine($"100{AppScreen.cur} X {hundredsZlotyCount} = {100* hundredsZlotyCount}");
        Console.WriteLine($"50{AppScreen.cur} X {fiftiesZlotyCount} = {50 * fiftiesZlotyCount}");
        Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

        int opt = Validator.Convert<int>("1 to confirm");

        return opt.Equals(1); //if one return true, else return false
    }
}

