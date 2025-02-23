export interface CreateShopModel{
    ShopId: string,
    Name: string,
    Description: string | null,
    LogoImage: File | null,
    ShopAddress: string,
    Phone: string,
    Email: string,
    TaxesCode: string,
    IdentificationNumber: string
}