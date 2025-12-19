import { useAuth } from 'nauth-react';
import { Link } from 'react-router-dom';
import { ROUTES } from '../lib/constants';
import { User, KeyRound, Calendar, Mail, Shield, Activity } from 'lucide-react';

export function DashboardPage() {
  const { user } = useAuth();

  const quickLinks = [
    {
      icon: <User className="w-6 h-6" />,
      title: 'View Profile',
      description: 'Manage your account information',
      to: ROUTES.PROFILE,
      color: 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400',
    },
    {
      icon: <KeyRound className="w-6 h-6" />,
      title: 'Change Password',
      description: 'Update your password',
      to: ROUTES.CHANGE_PASSWORD,
      color: 'bg-purple-100 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400',
    },
  ];

  const userStats = [
    {
      icon: <Mail className="w-5 h-5" />,
      label: 'Email',
      value: user?.email || 'N/A',
    },
    {
      icon: <Calendar className="w-5 h-5" />,
      label: 'Member Since',
      value: user?.createAt
        ? new Date(user.createAt).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
          })
        : 'N/A',
    },
    {
      icon: <Shield className="w-5 h-5" />,
      label: 'Account Status',
      value: 'Active',
    },
    {
      icon: <Activity className="w-5 h-5" />,
      label: 'User ID',
      value: user?.userId ? String(user.userId).substring(0, 8) + '...' : 'N/A',
    },
  ];

  return (
    <div className="max-w-6xl mx-auto space-y-8">
      {/* Welcome Section */}
      <div className="bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl p-8 text-white">
        <h1 className="text-3xl font-bold mb-2">Welcome back, {user?.name || user?.email}!</h1>
        <p className="text-blue-100">Manage your account and explore all features</p>
      </div>

      {/* User Stats Grid */}
      <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-4">
        {userStats.map((stat, index) => (
          <div
            key={index}
            className="bg-white dark:bg-gray-800 rounded-xl p-6 border border-gray-200 dark:border-gray-700"
          >
            <div className="flex items-center gap-3 mb-2">
              <div className="text-gray-500 dark:text-gray-400">{stat.icon}</div>
              <span className="text-sm text-gray-600 dark:text-gray-400">{stat.label}</span>
            </div>
            <p className="text-lg font-semibold dark:text-white truncate">{stat.value}</p>
          </div>
        ))}
      </div>

      {/* Quick Links */}
      <div>
        <h2 className="text-2xl font-bold mb-4 dark:text-white">Quick Actions</h2>
        <div className="grid md:grid-cols-2 gap-6">
          {quickLinks.map((link, index) => (
            <Link
              key={index}
              to={link.to}
              className="bg-white dark:bg-gray-800 rounded-xl p-6 border border-gray-200 dark:border-gray-700 hover:shadow-lg transition-all group"
            >
              <div className={`w-12 h-12 ${link.color} rounded-lg flex items-center justify-center mb-4 group-hover:scale-110 transition-transform`}>
                {link.icon}
              </div>
              <h3 className="text-lg font-semibold mb-2 dark:text-white">{link.title}</h3>
              <p className="text-gray-600 dark:text-gray-400">{link.description}</p>
            </Link>
          ))}
        </div>
      </div>

      {/* Account Info */}
      <div className="bg-white dark:bg-gray-800 rounded-xl p-6 border border-gray-200 dark:border-gray-700">
        <h2 className="text-xl font-bold mb-4 dark:text-white">Account Information</h2>
        <div className="space-y-4">
          <div className="flex items-center justify-between py-3 border-b border-gray-200 dark:border-gray-700">
            <span className="text-gray-600 dark:text-gray-400">Full Name</span>
            <span className="font-medium dark:text-white">{user?.name || 'Not set'}</span>
          </div>
          <div className="flex items-center justify-between py-3 border-b border-gray-200 dark:border-gray-700">
            <span className="text-gray-600 dark:text-gray-400">Email Address</span>
            <span className="font-medium dark:text-white">{user?.email}</span>
          </div>
          <div className="flex items-center justify-between py-3 border-b border-gray-200 dark:border-gray-700">
            <span className="text-gray-600 dark:text-gray-400">Account Status</span>
            <span className="px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400">
              Active
            </span>
          </div>
          <div className="flex items-center justify-between py-3">
            <span className="text-gray-600 dark:text-gray-400">User ID</span>
            <span className="font-mono text-sm dark:text-white">{user?.userId || 'N/A'}</span>
          </div>
        </div>
      </div>
    </div>
  );
}
