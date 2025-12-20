import React from 'react';
import { SearchForm } from 'nauth-react';
import type { PagedResult, UserInfo } from 'nauth-react';
import { useNavigate } from 'react-router-dom';

export default function SearchUsersPage() {
  const navigate = useNavigate();

  const handleSuccess = (result: PagedResult<UserInfo>) => {
    console.log('Search successful:', result);
  };

  const handleError = (error: Error) => {
    console.error('Search error:', error);
  };

  const handleUserClick = (user: UserInfo) => {
    console.log('User clicked:', user);
    // Navigate to user profile or open modal
    // navigate(`/users/${user.userId}`);
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
          Search Users
        </h1>
        <p className="mt-2 text-gray-600 dark:text-gray-400">
          Search and browse all users in the system
        </p>
      </div>

      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <SearchForm
          onSuccess={handleSuccess}
          onError={handleError}
          onUserClick={handleUserClick}
          initialPageSize={25}
          pageSizeOptions={[10, 25, 50, 100]}
          showUserAvatar={true}
        />
      </div>
    </div>
  );
}
