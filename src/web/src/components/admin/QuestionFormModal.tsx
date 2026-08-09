import { useEffect, useState } from 'react'
import { useForm, useFieldArray } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog, Flex, Text, TextField, Button, Callout, Select } from '@radix-ui/themes'
import { createQuestion, updateQuestion } from '../../services/questionService'
import type { QuestionDto } from '../../types/question'

const schema = z.object({
  content: z
    .string()
    .trim()
    .min(1, 'Question is required')
    .max(500, 'Question must be at most 500 characters'),
  questionType: z.string().default('MultipleChoice'),
  category: z.string().optional(),
  minAge: z
    .union([z.string(), z.number()])
    .optional()
    .transform((val) => (val ? parseInt(String(val), 10) : undefined))
    .refine((val) => !val || (val >= 0 && val <= 120), 'Age must be between 0 and 120'),
  options: z
    .array(
      z.object({
        label: z
          .string()
          .trim()
          .min(1, 'Option is required')
          .max(200, 'Option must be at most 200 characters'),
        isOther: z.boolean(),
        sortOrder: z.number(),
      })
    )
    .min(1, 'At least one option is required'),
})

type FormValues = z.infer<typeof schema>

interface QuestionFormModalProps {
  open: boolean
  mode: 'create' | 'edit'
  question?: QuestionDto | null
  onClose: () => void
  onSaved: () => void
}

export default function QuestionFormModal({
  open,
  mode,
  question,
  onClose,
  onSaved,
}: QuestionFormModalProps) {
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    reset,
    control,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema) as any,
    defaultValues: {
      content: '',
      questionType: 'MultipleChoice',
      category: '',
      minAge: undefined,
      options: [
        { label: '', isOther: false, sortOrder: 0 },
        { label: '', isOther: false, sortOrder: 1 },
        { label: '', isOther: false, sortOrder: 2 },
        { label: '', isOther: true, sortOrder: 3 },
      ],
    },
  })

  const { fields } = useFieldArray({
    control,
    name: 'options',
  })

  useEffect(() => {
    if (open) {
      setFormError(null)
      if (mode === 'edit' && question) {
        const options = question.options
          .sort((a, b) => a.sortOrder - b.sortOrder)
          .map((opt) => ({
            label: opt.label,
            isOther: opt.isOther,
            sortOrder: opt.sortOrder,
          }))

        reset({
          content: question.content,
          questionType: question.questionType,
          category: question.category || '',
          minAge: question.minAge,
          options:
            options.length >= 4
              ? options
              : [
                  ...options,
                  ...Array.from(
                    { length: 4 - options.length },
                    (_, i) => ({
                      label: '',
                      isOther: options.length + i === 3,
                      sortOrder: options.length + i,
                    })
                  ),
                ],
        })
      } else {
        reset({
          content: '',
          questionType: 'MultipleChoice',
          category: '',
          minAge: undefined,
          options: [
            { label: '', isOther: false, sortOrder: 0 },
            { label: '', isOther: false, sortOrder: 1 },
            { label: '', isOther: false, sortOrder: 2 },
            { label: '', isOther: true, sortOrder: 3 },
          ],
        })
      }
    }
  }, [open, mode, question, reset])

  async function onSubmit(values: FormValues) {
    setFormError(null)

    const request = {
      content: values.content.trim(),
      questionType: values.questionType,
      category: values.category ? values.category.trim() : undefined,
      minAge: values.minAge,
      options: values.options.map((opt) => ({
        label: opt.label.trim(),
        isOther: opt.isOther,
        sortOrder: opt.sortOrder,
      })),
    }

    try {
      if (mode === 'create') {
        await createQuestion(request)
      } else if (question) {
        await updateQuestion(question.id, request)
      }
      onSaved()
      onClose()
    } catch (error) {
      setFormError(error instanceof Error ? error.message : 'Something went wrong')
    }
  }

  return (
    <Dialog.Root open={open} onOpenChange={(next) => !next && onClose()}>
      <Dialog.Content maxWidth="500px">
        <Dialog.Title>{mode === 'create' ? 'Add New Question' : 'Edit Question'}</Dialog.Title>

        <form onSubmit={handleSubmit(onSubmit as any)}>
          <Flex direction="column" gap="3" mt="3" style={{ maxHeight: '60vh', overflowY: 'auto' }}>
            {formError && (
              <Callout.Root color="red">
                <Callout.Text>{formError}</Callout.Text>
              </Callout.Root>
            )}

            <label>
              <Text as="div" size="2" mb="1" weight="bold">
                Question Type
              </Text>
              <Select.Root defaultValue="MultipleChoice">
                <Select.Trigger />
                <Select.Content>
                  <Select.Item value="MultipleChoice">Multiple Choice</Select.Item>
                </Select.Content>
              </Select.Root>
              <input type="hidden" {...register('questionType')} value="MultipleChoice" />
            </label>

            <label>
              <Text as="div" size="2" mb="1" weight="bold">
                Question
              </Text>
              <textarea
                {...register('content')}
                placeholder="What is your question?"
                style={{
                  minHeight: '60px',
                  padding: '0.5rem',
                  borderRadius: '4px',
                  border: '1px solid #e5e7eb',
                  fontFamily: 'inherit',
                  fontSize: 'inherit',
                  width: '100%',
                  boxSizing: 'border-box',
                }}
              />
              {errors.content && (
                <Text color="red" size="1">
                  {errors.content.message}
                </Text>
              )}
            </label>

            <label>
              <Text as="div" size="2" mb="1" weight="bold">
                Category (Optional)
              </Text>
              <TextField.Root {...register('category')} placeholder="e.g., Preferences, Family" />
            </label>

            <label>
              <Text as="div" size="2" mb="1" weight="bold">
                Minimum Age (Optional)
              </Text>
              <TextField.Root type="number" {...register('minAge')} placeholder="e.g., 5" />
              {errors.minAge && (
                <Text color="red" size="1">
                  {errors.minAge.message}
                </Text>
              )}
            </label>

            <Flex direction="column" gap="2">
              <Text as="div" size="2" weight="bold">
                Answer Options
              </Text>
              {fields.map((field, index) => (
                <div key={field.id}>
                  <Flex align="center" gap="2">
                    <TextField.Root
                      {...register(`options.${index}.label`)}
                      placeholder={
                        index < 3
                          ? `Option ${index + 1}`
                          : 'Other (user can enter custom text)'
                      }
                      style={{ flex: 1 }}
                    />
                    {index < 3 && (
                      <Text size="1" color="gray">
                        Option {index + 1}
                      </Text>
                    )}
                    {index === 3 && (
                      <Text size="1" color="gray">
                        Other
                      </Text>
                    )}
                  </Flex>
                  {errors.options?.[index]?.label && (
                    <Text color="red" size="1">
                      {errors.options[index]?.label?.message}
                    </Text>
                  )}
                </div>
              ))}
            </Flex>
          </Flex>

          <Flex gap="3" mt="4" justify="end">
            <Dialog.Close>
              <Button type="button" variant="soft" color="gray">
                Cancel
              </Button>
            </Dialog.Close>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Saving…' : 'Save'}
            </Button>
          </Flex>
        </form>
      </Dialog.Content>
    </Dialog.Root>
  )
}
