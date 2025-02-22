using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class ShoppingCartItemRepository : BaseRepository<ShoppingCartItem>, IShoppingCartItemRepository
    {
        public ShoppingCartItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> AddNewShoppingCartItemAsync(ShoppingCartItem cartItem, int UserID)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var inventory = _context.Set<Inventory>().Where(g => g.ProductID == cartItem.ProductID && g.UnitChartID == cartItem.UnitChartID && cartItem.Quantity<g.Quantity).Any();
                    if (inventory)
                    {
                        Inventory inv = await _context.Set<Inventory>().Where(g => g.ProductID == cartItem.ProductID && g.UnitChartID == cartItem.UnitChartID).FirstOrDefaultAsync<Inventory>();
                        inv.Quantity -= cartItem.Quantity;
                        cartItem.UnitPrice = inv.SellingPrice;

                        var shoppingCartExists = _context.Set<ShoppingCart>().Where(x => x.UserID == UserID && x.ShoppingCartStatus == EShopModels.Common.ShoppingCartStatus.Pending).Any();
                        if (shoppingCartExists)
                        {
                            ShoppingCart cart = await _context.Set<ShoppingCart>().Where(x => x.UserID == UserID && x.ShoppingCartStatus == EShopModels.Common.ShoppingCartStatus.Pending).FirstOrDefaultAsync();
                            cartItem.ShoppingCartID = cart.ID;
                            var cartItemExists = _context.Set<ShoppingCartItem>().Where(t => t.ShoppingCartID == cart.ID && t.UnitChartID == cartItem.UnitChartID && t.ProductID == cartItem.ProductID).Any();
                            if (cartItemExists)
                            {
                                ShoppingCartItem shoppingcartitem = await _context.Set<ShoppingCartItem>().Where(t => t.ShoppingCartID == cart.ID && t.UnitChartID == cartItem.UnitChartID && t.ProductID == cartItem.ProductID).FirstOrDefaultAsync();
                                shoppingcartitem.Quantity += cartItem.Quantity;
                                _context.MarkAsModified(shoppingcartitem);
                            }
                            else
                            {
                                await Set.AddAsync(cartItem);
                            }
                            cart.GrossAmount = cart.GrossAmount + (cartItem.Quantity * cartItem.UnitPrice) - cartItem.LineDiscount;
                            _context.MarkAsModified(cart);
                        }
                        else
                        {
                            ShoppingCart shoppingCart = new ShoppingCart(UserID, 0, 0, 0) { CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, ShoppingCartStatus = EShopModels.Common.ShoppingCartStatus.Pending };
                            shoppingCart.ShoppingCartItems.Add(cartItem);
                            shoppingCart.GrossAmount = shoppingCart.GrossAmount + (cartItem.Quantity * cartItem.UnitPrice) - cartItem.LineDiscount;
                            await _context.Set<ShoppingCart>().AddAsync(shoppingCart);
                        }

                        //await _context.SaveChangesAsync();
                        //await transaction.CommitAsync();
                        return await Task.FromResult<object>(cartItem);
                    }
                    else
                    {
                        throw new Exception("Insufficient Quantity");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Task.FromException(ex);
                }
            }
        }

        public async Task<object> DeleteCartItemAsync(int id)
        {
            int result = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                { 
                    ValueTask<ShoppingCartItem?> cartItem = Set.FindAsync(id);
                    if (cartItem.Result != null)
                    {
                        Inventory? inventory = await _context.Set<Inventory>().Where(g => g.ProductID == cartItem.Result.ProductID && g.UnitChartID == cartItem.Result.UnitChartID).FirstOrDefaultAsync<Inventory>();
                        inventory.Quantity += cartItem.Result.Quantity;
                        _context.MarkAsDeleted(cartItem.Result);
                        result = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    return await Task.FromResult<object>(result);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return await Task.FromException<object>(ex);
                }
            }
        }

        public async Task<object> UpdateCartItemQtyAsync(ShoppingCartItem shoppingCartItem)
        {
            int result = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ValueTask<ShoppingCartItem?> cartItem = Set.FindAsync(shoppingCartItem.ID);                   
                    if (cartItem.Result != null)
                    {
                        ShoppingCart cart = await _context.Set<ShoppingCart>().Where(g => g.ID == cartItem.Result.ShoppingCartID).FirstAsync();
                        Inventory? inventory = await _context.Set<Inventory>().Where(g => g.ProductID == cartItem.Result.ProductID && g.UnitChartID == cartItem.Result.UnitChartID).FirstOrDefaultAsync<Inventory>();
                        inventory.Quantity = inventory.Quantity - (shoppingCartItem.Quantity - cartItem.Result.Quantity);
                        cart.GrossAmount = cart.GrossAmount + ((shoppingCartItem.Quantity - cartItem.Result.Quantity) * cartItem.Result.UnitPrice) - cartItem.Result.LineDiscount;
                        cartItem.Result.Quantity = shoppingCartItem.Quantity;                       
                        _context.MarkAsModified(cartItem.Result);
                        _context.MarkAsModified(cart);
                        _context.MarkAsModified(inventory);
                        result = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    return await Task.FromResult<object>(result);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return await Task.FromException<object>(ex);
                }
            }
        } 
    }
}
