import { useEffect, useState } from 'react'
import { Table, Heading, Badge } from '@radix-ui/themes'
import { getAllUsers } from '../../services/userService'
import type { UserDto } from '../../types/user'

export default function AdminUsersPage() {
  const [users, setUsers] = useState<UserDto[]>([])

  useEffect(() => {
    getAllUsers().then(setUsers)
  }, [])

  return (
    <div>
      <Heading mb="4">Users</Heading>
      <Table.Root variant="surface">
        <Table.Header>
          <Table.Row>
            <Table.ColumnHeaderCell>Name</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Roles</Table.ColumnHeaderCell>
            <Table.ColumnHeaderCell>Created</Table.ColumnHeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {users.map((user) => (
            <Table.Row key={user.id}>
              <Table.Cell>{user.displayName}</Table.Cell>
              <Table.Cell>
                {user.roles.map((role) => (
                  <Badge key={role} mr="1">
                    {role}
                  </Badge>
                ))}
              </Table.Cell>
              <Table.Cell>{new Date(user.createdAt).toLocaleDateString()}</Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table.Root>
    </div>
  )
}
