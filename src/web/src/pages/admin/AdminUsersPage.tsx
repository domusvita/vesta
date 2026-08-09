import { useEffect, useMemo, useState } from 'react'
import { Table, Heading, Badge, Button, Flex, AlertDialog, Callout } from '@radix-ui/themes'
import { getAllUsers, deleteUser } from '../../services/userService'
import type { UserDto } from '../../types/user'
import Pagination from '../../components/Pagination'
import UserFormModal from '../../components/admin/UserFormModal'

export default function AdminUsersPage() {
  const [users, setUsers] = useState<UserDto[]>([])
  const [loadError, setLoadError] = useState<string | null>(null)

  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState<number>(10)

  const [modalOpen, setModalOpen] = useState(false)
  const [modalMode, setModalMode] = useState<'create' | 'edit'>('create')
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null)

  const [userPendingDelete, setUserPendingDelete] = useState<UserDto | null>(null)
  const [deleteError, setDeleteError] = useState<string | null>(null)

  function refresh() {
    setLoadError(null)
    getAllUsers()
      .then(setUsers)
      .catch(() => setLoadError('Failed to load users'))
  }

  useEffect(() => {
    refresh()
  }, [])

  const pagedUsers = useMemo(() => {
    if (pageSize === Infinity) {
      return users
    }
    const start = (page - 1) * pageSize
    return users.slice(start, start + pageSize)
  }, [users, page, pageSize])

  useEffect(() => {
    setPage(1)
  }, [pageSize])

  function openCreateModal() {
    setModalMode('create')
    setSelectedUser(null)
    setModalOpen(true)
  }

  function openEditModal(user: UserDto) {
    setModalMode('edit')
    setSelectedUser(user)
    setModalOpen(true)
  }

  async function confirmDelete() {
    if (!userPendingDelete) return
    setDeleteError(null)
    try {
      await deleteUser(userPendingDelete.id)
      setUserPendingDelete(null)
      refresh()
    } catch (error) {
      setDeleteError(error instanceof Error ? error.message : 'Failed to remove user access')
    }
  }

  return (
    <div>
      <Flex align="center" justify="between" mb="4">
        <Heading>Users</Heading>
        <Button onClick={openCreateModal}>Add New User</Button>
      </Flex>

      {loadError && (
        <Callout.Root color="red" mb="3">
          <Callout.Text>{loadError}</Callout.Text>
        </Callout.Root>
      )}

      <Table.Root variant="surface">
        <Table.Header>
          <Table.Row>
            <Table.ColumnHeaderCell>Name</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Roles</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Date of Birth</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Created</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell></Table.ColumnHeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {pagedUsers.map((user) => (
            <Table.Row
              key={user.id}
              onClick={() => openEditModal(user)}
              style={{ cursor: 'pointer' }}
            >
              <Table.Cell>{user.displayName}</Table.Cell>
              <Table.Cell>
                {user.roles.map((role) => (
                  <Badge key={role.id} mr="1">
                    {role.name}
                  </Badge>
                ))}
              </Table.Cell>
              <Table.Cell>
                {user.dateOfBirth
                  ? new Date(user.dateOfBirth).toLocaleDateString(undefined, { timeZone: 'UTC' })
                  : '—'}
              </Table.Cell>
              <Table.Cell>{new Date(user.createdAt).toLocaleDateString()}</Table.Cell>
              <Table.Cell>
                <Button
                  type="button"
                  variant="ghost"
                  color="red"
                  aria-label="Remove user access"
                  onClick={(event) => {
                    event.stopPropagation()
                    setDeleteError(null)
                    setUserPendingDelete(user)
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
        totalItems={users.length}
        onPageChange={setPage}
        onPageSizeChange={setPageSize}
      />

      <UserFormModal
        open={modalOpen}
        mode={modalMode}
        user={selectedUser}
        onClose={() => setModalOpen(false)}
        onSaved={refresh}
      />

      <AlertDialog.Root
        open={Boolean(userPendingDelete)}
        onOpenChange={(next) => !next && setUserPendingDelete(null)}
      >
        <AlertDialog.Content maxWidth="450px">
          <AlertDialog.Title>Remove user access</AlertDialog.Title>
          <AlertDialog.Description>
            Are you sure you want to remove{' '}
            <strong>{userPendingDelete?.displayName}</strong>&apos;s roles? This removes their
            access to the app but does not delete their account or data.
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
            <Button color="red" onClick={confirmDelete}>
              Remove access
            </Button>
          </Flex>
        </AlertDialog.Content>
      </AlertDialog.Root>
    </div>
  )
}
