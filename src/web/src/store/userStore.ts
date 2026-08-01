import { create } from 'zustand'
import type { UserDto } from '../types/user'

interface UserState {
  user: UserDto | null
  setUser: (user: UserDto) => void
  clearUser: () => void
}

const useUserStore = create<UserState>((set) => ({
  user: null,
  setUser: (user) => set({ user }),
  clearUser: () => set({ user: null }),
}))

export default useUserStore
