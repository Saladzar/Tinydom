using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int startingCrystalAmount { get; private set; }
    public int currentCrystalAmount { get; private set; }
    public int crystalPerStage { get; private set; }
    public int minePrice { get; private set; }
    public int destroyPrice { get; private set; }
    public bool isMined { get; private set; }
    public bool isLarge { get; private set; }

    public void InitializeCrystal(int amount, bool isLarge)
    {
        startingCrystalAmount = amount;
        currentCrystalAmount = startingCrystalAmount;
        this.isLarge = isLarge;
        isMined = false;
    }

    public void SetIsMined(bool isMined)
    {
        this.isMined = isMined;
    }

    public int RemoveCrystals(int amount)
    {
        if (amount >= currentCrystalAmount)
        {
            DestroyCrystal();
            return currentCrystalAmount;
        }

        currentCrystalAmount -= amount;
        return amount;
    }

    private void DestroyCrystal()
    {
        CrystalManager.Instance.CrystalDestroyed(startingCrystalAmount);
        CrystalManager.Instance.RemoveCrystal(this);
        Destroy(gameObject);
    }
}
