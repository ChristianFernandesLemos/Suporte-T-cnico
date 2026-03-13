import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

const PriorityBadge = ({ priority }) => {
  const getPriorityStyle = () => {
    switch (priority.toLowerCase()) {
      case 'baixa':
        return { backgroundColor: '#4CAF50', color: '#fff' };
      case 'média':
        return { backgroundColor: '#FFC107', color: '#000' };
      case 'alta':
        return { backgroundColor: '#F44336', color: '#fff' };
      default:
        return { backgroundColor: '#9E9E9E', color: '#fff' };
    }
  };

  const style = getPriorityStyle();

  return (
    <View style={[styles.badge, { backgroundColor: style.backgroundColor }]}>
      <Text style={[styles.text, { color: style.color }]}>{priority}</Text>
    </View>
  );
};

const styles = StyleSheet.create({
  badge: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 6,
    alignSelf: 'flex-start',
  },
  text: {
    fontSize: 14,
    fontWeight: '600',
  },
});

export default PriorityBadge;

