import axios from 'axios'
import apiClient from './apiClient'
import type { QuestionDto, CreateQuestionRequest, UpdateQuestionRequest } from '../types/question'

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

export async function getAllQuestions(): Promise<QuestionDto[]> {
  try {
    const response = await apiClient.get<FunctionResponse<QuestionDto[]>>('/api/questions')
    return response.data.data ?? []
  } catch (error) {
    throw new Error(extractErrorMessage(error, 'Failed to load questions'))
  }
}

export async function createQuestion(request: CreateQuestionRequest): Promise<QuestionDto> {
  try {
    const response = await apiClient.post<FunctionResponse<QuestionDto>>('/api/questions', request)

    if (!response.data.success || !response.data.data) {
      throw new Error(response.data.message ?? 'Failed to create question')
    }

    return response.data.data
  } catch (error) {
    throw new Error(extractErrorMessage(error, 'Failed to create question'))
  }
}

export async function updateQuestion(id: string, request: UpdateQuestionRequest): Promise<QuestionDto> {
  try {
    const response = await apiClient.put<FunctionResponse<QuestionDto>>(`/api/questions/${id}`, request)

    if (!response.data.success || !response.data.data) {
      throw new Error(response.data.message ?? 'Failed to update question')
    }

    return response.data.data
  } catch (error) {
    throw new Error(extractErrorMessage(error, 'Failed to update question'))
  }
}

export async function deleteQuestion(id: string): Promise<QuestionDto> {
  try {
    const response = await apiClient.delete<FunctionResponse<QuestionDto>>(`/api/questions/${id}`)

    if (!response.data.success || !response.data.data) {
      throw new Error(response.data.message ?? 'Failed to delete question')
    }

    return response.data.data
  } catch (error) {
    throw new Error(extractErrorMessage(error, 'Failed to delete question'))
  }
}
