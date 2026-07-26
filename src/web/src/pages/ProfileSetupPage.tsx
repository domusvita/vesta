import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useNavigate } from 'react-router-dom'
import { register } from '../services/userService'
import useUserStore from '../store/userStore'

const schema = z.object({
  displayName: z
    .string()
    .min(2, 'Display name must be at least 2 characters')
    .max(100, 'Display name must be at most 100 characters'),
})

type FormValues = z.infer<typeof schema>

export default function ProfileSetupPage() {
  const navigate = useNavigate()
  const setUser = useUserStore((s) => s.setUser)

  const {
    register: registerField,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
  })

  async function onSubmit(values: FormValues) {
    const user = await register(values.displayName)
    setUser(user)
    navigate('/')
  }

  return (
    <div>
      <h1>Set up your profile</h1>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label htmlFor="displayName">Display name</label>
          <input id="displayName" {...registerField('displayName')} />
          {errors.displayName && <p>{errors.displayName.message}</p>}
        </div>
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Setting up…' : 'Continue'}
        </button>
      </form>
    </div>
  )
}
