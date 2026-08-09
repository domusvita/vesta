import { useEffect, useMemo, useState } from 'react'
import { Table, Heading, Badge, Button, Flex, AlertDialog, Callout } from '@radix-ui/themes'
import { getAllQuestions, deleteQuestion } from '../../services/questionService'
import type { QuestionDto } from '../../types/question'
import Pagination from '../../components/Pagination'
import QuestionFormModal from '../../components/admin/QuestionFormModal'

export default function AdminQuestionsPage() {
  const [questions, setQuestions] = useState<QuestionDto[]>([])
  const [loadError, setLoadError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState<number>(10)

  const [modalOpen, setModalOpen] = useState(false)
  const [modalMode, setModalMode] = useState<'create' | 'edit'>('create')
  const [selectedQuestion, setSelectedQuestion] = useState<QuestionDto | null>(null)

  const [questionPendingDelete, setQuestionPendingDelete] = useState<QuestionDto | null>(null)
  const [deleteError, setDeleteError] = useState<string | null>(null)

  function refresh() {
    setLoadError(null)
    getAllQuestions()
      .then((loaded) => {
        setQuestions(loaded.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()))
      })
      .catch(() => setLoadError('Failed to load questions'))
  }

  useEffect(() => {
    refresh()
  }, [])

  const pagedQuestions = useMemo(() => {
    if (pageSize === Infinity) {
      return questions
    }
    const start = (page - 1) * pageSize
    return questions.slice(start, start + pageSize)
  }, [questions, page, pageSize])

  useEffect(() => {
    setPage(1)
  }, [pageSize])

  function openCreateModal() {
    setModalMode('create')
    setSelectedQuestion(null)
    setSuccessMessage(null)
    setModalOpen(true)
  }

  function openEditModal(question: QuestionDto) {
    setModalMode('edit')
    setSelectedQuestion(question)
    setSuccessMessage(null)
    setModalOpen(true)
  }

  async function confirmDelete() {
    if (!questionPendingDelete) return
    setDeleteError(null)
    try {
      await deleteQuestion(questionPendingDelete.id)
      setQuestionPendingDelete(null)
      setSuccessMessage('Question deleted successfully.')
      setTimeout(() => setSuccessMessage(null), 4000)
      refresh()
    } catch (error) {
      setDeleteError(error instanceof Error ? error.message : 'Failed to delete question')
    }
  }

  function handleModalSaved() {
    setSuccessMessage('Question saved successfully.')
    setTimeout(() => setSuccessMessage(null), 4000)
    refresh()
  }

  return (
    <div>
      <Flex align="center" justify="between" mb="4">
        <Heading>Questions</Heading>
        <Button onClick={openCreateModal}>Add New Question</Button>
      </Flex>

      {loadError && (
        <Callout.Root color="red" mb="3">
          <Callout.Text>{loadError}</Callout.Text>
        </Callout.Root>
      )}

      {successMessage && (
        <Callout.Root color="green" mb="3">
          <Callout.Text>{successMessage}</Callout.Text>
        </Callout.Root>
      )}

      <Table.Root variant="surface">
        <Table.Header>
          <Table.Row>
            <Table.ColumnHeaderCell>Question</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Type</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Category</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Min Age</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Created</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell></Table.ColumnHeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {pagedQuestions.map((question) => (
            <Table.Row
              key={question.id}
              onClick={() => openEditModal(question)}
              style={{ cursor: 'pointer' }}
            >
              <Table.Cell>
                <div style={{ maxWidth: '300px', wordWrap: 'break-word' }}>
                  {question.content}
                </div>
              </Table.Cell>
              <Table.Cell>
                <Badge>{question.questionType}</Badge>
              </Table.Cell>
              <Table.Cell>{question.category || '—'}</Table.Cell>
              <Table.Cell>{question.minAge ?? '—'}</Table.Cell>
              <Table.Cell>{new Date(question.createdAt).toLocaleDateString()}</Table.Cell>
              <Table.Cell>
                <Button
                  type="button"
                  variant="ghost"
                  color="red"
                  aria-label="Delete question"
                  onClick={(event) => {
                    event.stopPropagation()
                    setDeleteError(null)
                    setQuestionPendingDelete(question)
                  }}
                >
                  🗑
                </Button>
              </Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table.Root>

      <Pagination
        page={page}
        pageSize={pageSize}
        totalItems={questions.length}
        onPageChange={setPage}
        onPageSizeChange={setPageSize}
      />

      <QuestionFormModal
        open={modalOpen}
        mode={modalMode}
        question={selectedQuestion}
        onClose={() => setModalOpen(false)}
        onSaved={handleModalSaved}
      />

      <AlertDialog.Root
        open={Boolean(questionPendingDelete)}
        onOpenChange={(next) => !next && setQuestionPendingDelete(null)}
      >
        <AlertDialog.Content maxWidth="450px">
          <AlertDialog.Title>Delete Question</AlertDialog.Title>
          <AlertDialog.Description>
            Are you sure you want to delete this question?
            <br />
            <strong>{questionPendingDelete?.content}</strong>
          </AlertDialog.Description>

          {deleteError && (
            <Callout.Root color="red" mt="3">
              <Callout.Text>{deleteError}</Callout.Text>
            </Callout.Root>
          )}

          <Flex gap="3" mt="4" justify="end">
            <AlertDialog.Cancel>
              <Button variant="soft" color="gray">
                Cancel
              </Button>
            </AlertDialog.Cancel>
            <AlertDialog.Action onClick={confirmDelete}>
              <Button color="red">Delete</Button>
            </AlertDialog.Action>
          </Flex>
        </AlertDialog.Content>
      </AlertDialog.Root>
    </div>
  )
}
