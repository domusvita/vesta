import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Dialog, Flex, Text, TextField, Button, Callout } from '@radix-ui/themes'
import { createUser, updateUser } from '../../services/userService'
import type { UserDto } from '../../types/user'

function isTodayOrEarlier(dateStr: string): boolean {
  const date = new Date(dateStr)
  const today = new Date()
  today.setHours(23, 59, 59, 999)
  return date.getTime() <= today.getTime()
}

const schema = z.object({
  displayName: z
    .string()
    .trim()
    .min(2, 'Display name must be at least 2 characters')
    .max(100, 'Display name must be at most 100 characters'),
  dateOfBirth: z
    .string()
    .optional()
    .refine((value) => !value || isTodayOrEarlier(value), {
      message: 'Date of birth cannot be in the future',
    }),
  avatarUrl: z
    .string()
    .optional()
    .refine((value) => !value || z.string().url().safeParse(value).success, {
      message: 'Avatar URL must be a valid URL',
    })
    .refine((value) => !value || /^https?:\/\//i.test(value), {
      message: 'Avatar URL must start with http:// or https://',
    }),
})

type FormValues = z.infer<typeof schema>

interface UserFormModalProps {
  open: boolean
  mode: 'create' | 'edit'
  user?: UserDto | null
  onClose: () => void
  onSaved: () => void
}

export default function UserFormModal({ open, mode, user, onClose, onSaved }: UserFormModalProps) {
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      displayName: '',
      dateOfBirth: '',
      avatarUrl: '',
    },
  })

  const avatarUrl = watch('avatarUrl')
  const [previewFailed, setPreviewFailed] = useState(false)

  useEffect(() => {
    if (open) {
      setFormError(null)
      setPreviewFailed(false)
      reset({
        displayName: user?.displayName ?? '',
        dateOfBirth: user?.dateOfBirth ? user.dateOfBirth.slice(0, 10) : '',
        avatarUrl: user?.avatarUrl ?? '',
      })
    }
  }, [open, user, reset])

  async function onSubmit(values: FormValues) {
    setFormError(null)
    const request = {
      displayName: values.displayName.trim(),
      dateOfBirth: values.dateOfBirth ? values.dateOfBirth : null,
      avatarUrl: values.avatarUrl ? values.avatarUrl : null,
    }

    try {
      if (mode === 'create') {
        await createUser(request)
      } else if (user) {
        await updateUser(user.id, request)
      }
      onSaved()
      onClose()
    } catch (error) {
      setFormError(error instanceof Error ? error.message : 'Something went wrong')
    }
  }

  const showPreview = Boolean(avatarUrl) && !previewFailed

  return (
    <Dialog.Root open={open} onOpenChange={(next) => !next && onClose()}>
      <Dialog.Content maxWidth="450px">
        <Dialog.Title>{mode === 'create' ? 'Add New User' : 'Edit User'}</Dialog.Title>

        <form onSubmit={handleSubmit(onSubmit)}>
          <Flex direction="column" gap="3" mt="3">
            {formError && (
              <Callout.Root color="red">
                <Callout.Text>{formError}</Callout.Text>
              </Callout.Root>
            )}

            <label>
              <Text as="div" size="2" mb="1" weight="bold">
                Display Name
              </Text>
              <TextField.Root {...register('displayName')} placeholder="Display name" />
              {errors.displayName && (
                <Text color="red" size="1">
                  {errors.displayName.message}
                </Text>
              )}
            </label>

            <label>
              <Text as="div" size="2" mb="1" weight="bold">
                Date of Birth
              </Text>
              <TextField.Root type="date" {...register('dateOfBirth')} />
              {errors.dateOfBirth && (
                <Text color="red" size="1">
                  {errors.dateOfBirth.message}
                </Text>
              )}
            </label>

            <label>
              <Text as="div" size="2" mb="1" weight="bold">
                Avatar URL
              </Text>
              <TextField.Root
                type="url"
                {...register('avatarUrl', {
                  onChange: () => setPreviewFailed(false),
                })}
                placeholder="https://example.com/avatar.png"
              />
              {errors.avatarUrl && (
                <Text color="red" size="1">
                  {errors.avatarUrl.message}
                </Text>
              )}
            </label>

            {showPreview && (
              <img
                src={avatarUrl}
                alt="Avatar preview"
                width={64}
                height={64}
                style={{ objectFit: 'cover', borderRadius: '50%' }}
                onError={() => setPreviewFailed(true)}
              />
            )}
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
