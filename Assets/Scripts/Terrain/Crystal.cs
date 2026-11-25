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
    private bool isDestroyed;

    public void InitializeCrystal(int amount, bool isLarge, int crystalPerStage, int minePrice, int destroyPrice)
    {
        startingCrystalAmount = amount;
        currentCrystalAmount = startingCrystalAmount;
        this.isLarge = isLarge;
        this.crystalPerStage = crystalPerStage;
        this.minePrice = minePrice;
        this.destroyPrice = destroyPrice;
        isMined = false;
    }

    public void SetIsMined(bool isMined)
    {
        this.isMined = isMined;
    }

    public int RemoveCrystals(int amount)
    {
        if (amount <= 0 || currentCrystalAmount <= 0) return 0;

        int removedAmount = Mathf.Min(amount, currentCrystalAmount);
        currentCrystalAmount -= removedAmount;
        if (currentCrystalAmount == 0) DestroyCrystal();

        return removedAmount;
    }

    public void DestroyCrystal()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        GridTile tile = GridManager.Instance.GetTileFromWorldPosition(transform.position);
        if (tile != null && tile.crystal == this)
            tile.crystal = null;

        CrystalManager.Instance.CrystalDestroyed(startingCrystalAmount);
        CrystalManager.Instance.RemoveCrystal(this);
        Destroy(gameObject);
    }
}
