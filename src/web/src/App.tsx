import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Theme } from '@radix-ui/themes'
import MainLayout from './layouts/MainLayout'
import HomePage from './pages/HomePage'
import NotFoundPage from './pages/NotFoundPage'

export default function App() {
  return (
    <Theme appearance="light" accentColor="indigo" radius="medium">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<MainLayout />}>
            <Route index element={<HomePage />} />
            <Route path="*" element={<NotFoundPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </Theme>
  )
}
