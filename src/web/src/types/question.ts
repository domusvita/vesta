export interface QuestionOptionDto {
  id: string
  label: string
  isOther: boolean
  sortOrder: number
}

export interface QuestionDto {
  id: string
  content: string
  questionType: string
  category?: string
  minAge?: number
  isActive: boolean
  createdAt: string
  options: QuestionOptionDto[]
}

export interface CreateQuestionRequest {
  content: string
  questionType: string
  category?: string
  minAge?: number
  options: CreateQuestionOptionRequest[]
}

export interface CreateQuestionOptionRequest {
  label: string
  isOther: boolean
  sortOrder: number
}

export interface UpdateQuestionRequest {
  content: string
  questionType: string
  category?: string
  minAge?: number
  options: UpdateQuestionOptionRequest[]
}

export interface UpdateQuestionOptionRequest {
  id?: string
  label: string
  isOther: boolean
  sortOrder: number
}
