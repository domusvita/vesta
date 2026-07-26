import apiClient from './apiClient'
import type { UserDto } from '../types/user'

interface FunctionResponse<T> {
  success: boolean
  data?: T
}

export async function getMe(): Promise<UserDto | null> {
  const response = await apiClient.get<FunctionResponse<UserDto>>('/api/users/me')
  return response.data.data ?? null
}

export async function register(displayName: string): Promise<UserDto> {
  const response = await apiClient.post<FunctionResponse<UserDto>>('/api/users/register', {
    displayName,
  })

  if (!response.data.success || !response.data.data) {
    throw new Error('Registration failed')
  }

  return response.data.data
}

export async function getAllUsers(): Promise<UserDto[]> {
  const response = await apiClient.get<FunctionResponse<UserDto[]>>('/api/users')
  return response.data.data ?? []
}
