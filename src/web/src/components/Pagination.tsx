import { Button, Flex, Select, Text } from '@radix-ui/themes'

const PAGE_SIZE_OPTIONS = [10, 25, 50] as const

interface PaginationProps {
  page: number
  pageSize: number
  totalItems: number
  onPageChange: (page: number) => void
  onPageSizeChange: (pageSize: number) => void
}

export default function Pagination({
  page,
  pageSize,
  totalItems,
  onPageChange,
  onPageSizeChange,
}: PaginationProps) {
  const isAll = pageSize >= totalItems || pageSize === Infinity
  const totalPages = isAll ? 1 : Math.max(1, Math.ceil(totalItems / pageSize))
  const currentPage = Math.min(page, totalPages)

  return (
    <Flex align="center" justify="between" mt="3" gap="3">
      <Flex align="center" gap="2">
        <Text size="2">Rows per page</Text>
        <Select.Root
          value={pageSize === Infinity ? 'all' : String(pageSize)}
          onValueChange={(value) => onPageSizeChange(value === 'all' ? Infinity : Number(value))}
        >
          <Select.Trigger />
          <Select.Content>
            {PAGE_SIZE_OPTIONS.map((size) => (
              <Select.Item key={size} value={String(size)}>
                {size}
              </Select.Item>
            ))}
            <Select.Item value="all">All</Select.Item>
          </Select.Content>
        </Select.Root>
      </Flex>
      <Flex align="center" gap="3">
        <Text size="2">
          Page {currentPage} of {totalPages}
        </Text>
        <Button
          type="button"
          variant="soft"
          disabled={currentPage <= 1}
          onClick={() => onPageChange(currentPage - 1)}
        >
          Previous
        </Button>
        <Button
          type="button"
          variant="soft"
          disabled={currentPage >= totalPages}
          onClick={() => onPageChange(currentPage + 1)}
        >
          Next
        </Button>
      </Flex>
    </Flex>
  )
}
