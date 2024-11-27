export interface RoleResModel {
    roleId: string
    roleName: string
    claims: Claim[]
  }
  
export interface Claim {
    claimId: number
    claimType: string
    claimValue: string
}