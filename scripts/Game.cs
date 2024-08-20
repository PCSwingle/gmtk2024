using System;
using System.Collections.Generic;
using Godot;

namespace GMTK2024.scripts;

public class Game {
    public const float PriceMultiplier = 1.11f;
    public const int DefaultDemand = 100;
    public const float DemandModifier = 0.10f;

    // Simply take this to the power of the current demand balance and multiply by the base price to get the total price :D
    public static readonly float PriceMultiplierPerExtraDemand = MathF.Pow(
        PriceMultiplier,
        1f / (DefaultDemand * DemandModifier)
    );

    public static readonly Game State = new();

    public float Coins = 100000;

    // MARKET DETAILS:
    // Start at base price, order balance equal to half of the total balance of the last tick (leading to exponential decay)
    // Price is adjusted based on order balance; 10% higher demand = 11% higher price
    // You buy at current price (good deal) and sell at lowered price (bad deal), evening out and preventing infinite money
    // P = P_base * P_multiplier ^ (D_balance / (D_default * D_modifier))
    // P = Price, P_base = base material price, P_multiplier = Price multiplier = 1.11
    // D_balance = Current demand balance (wholly set by player), D_default = Default demand (higher value leads to less elastic market, change over time?), D_modifier = Demand modifier = 0.10
    // To get the price of buying n resources starting at price P, you can simplify and get a geometric series that resolves to P_total = P_initial * (k^n - 1) / (k - 1) where k = PriceMultiplierPerExtraDemand

    private readonly Dictionary<Resource, int> _balanceSheet = [];


    private float _PriceAtDemand(
        float basePrice,
        int demandBalance
    ) {
        return basePrice * MathF.Pow(
            PriceMultiplierPerExtraDemand,
            demandBalance
        );
    }

    private float _TotalPrice(
        float basePrice,
        int currentDemand,
        int orderSize,
        bool buy
    ) {
        // This means you get a good deal on buying (you buy, and then it raises price) but a bad deal on selling (it lowers price before you sell), so it evens out in the end
        var initialPrice = this._PriceAtDemand(
            basePrice,
            currentDemand - (buy ? 0 : orderSize)
        );
        var totalPrice = initialPrice * (Mathf.Pow(
            PriceMultiplierPerExtraDemand,
            orderSize
        ) - 1) / (PriceMultiplierPerExtraDemand - 1);
        return totalPrice;
    }

    public float Buy(
        Resource res,
        int amount,
        bool query
    ) {
        // todo
        return 0;
    }

    public float Sell(
        Resource res,
        int amount,
        bool query
    ) {
        // todo
        return 0;
    }
}