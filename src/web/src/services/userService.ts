import axios from 'axios'
import apiClient from './apiClient'
import type { UpsertUserRequest, UserDto } from '../types/user'

interface FunctionResponse<T> {
  success: boolean
  data?: T
  message?: string
}

function extractErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error)) {
    const message = (error.response?.data as FunctionResponse<unknown> | undefined)?.message
    if (message) {
      return message
    }
  }

  if (error instanceof Error && error.message) {
    return error.message
  }

  return fallback
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

export async function createUser(request: UpsertUserRequest): Promise<UserDto> {
  try {
    const response = await apiClient.post<FunctionResponse<UserDto>>('/api/users/admin', request)

    if (!response.data.success || !response.data.data) {
      throw new Error(response.data.message ?? 'Failed to create user')
    }

    return response.data.data
  } catch (error) {
    throw new Error(extractErrorMessage(error, 'Failed to create user'))
  }
}

export async function updateUser(id: string, request: UpsertUserRequest): Promise<UserDto> {
  try {
    const response = await apiClient.put<FunctionResponse<UserDto>>(`/api/users/${id}`, request)

    if (!response.data.success || !response.data.data) {
      throw new Error(response.data.message ?? 'Failed to update user')
    }

    return response.data.data
  } catch (error) {
    throw new Error(extractErrorMessage(error, 'Failed to update user'))
  }
}

export async function deleteUser(id: string): Promise<UserDto> {
  try {
    const response = await apiClient.delete<FunctionResponse<UserDto>>(`/api/users/${id}`)

    if (!response.data.success || !response.data.data) {
      throw new Error(response.data.message ?? 'Failed to remove user roles')
    }

    return response.data.data
  } catch (error) {
    throw new Error(extractErrorMessage(error, 'Failed to remove user roles'))
  }
}
