export interface UserDto {
  id: string
  displayName: string
  dateOfBirth: string | null
  avatarUrl: string | null
  roles: string[]
  createdAt: string
}

export interface UpsertUserRequest {
  displayName: string
  dateOfBirth: string | null
  avatarUrl: string | null
}
