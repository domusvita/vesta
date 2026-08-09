export interface RoleDto {
  id: string
  name: string
}

export interface UserDto {
  id: string
  displayName: string
  avatarUrl: string
  roles: RoleDto[]
  createdAt: string
}
